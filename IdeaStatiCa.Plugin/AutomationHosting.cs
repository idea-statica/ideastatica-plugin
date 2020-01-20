using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Responsible for controlling the connected BIM application to IS
	/// </summary>
	/// <typeparam name="T">Type of the plugin's service contract</typeparam>
	public interface IBIMPluginClient<T>
	{
		/// <summary>
		/// Used for calling methods of connected BIM application
		/// </summary>
		T MyBIM { get; }

		/// <summary>
		/// Notification about events in the connected BIM application
		/// </summary>
		event ISEventHandler BIMStatusChanged;

		/// <summary>
		/// Starts BIM application
		/// </summary>
		/// <param name="id">Identified of </param>
		/// <returns></returns>
		Task RunAsync(string id);

		/// <summary>
		/// Stops BIM application
		/// </summary>
		void Stop();
	}

	/// <summary>
	/// Responsible of hosting an automation service on net.pipe endpoint
	/// </summary>
	/// <typeparam name="MyInterface"></typeparam>
	/// <typeparam name="ClientInterface"></typeparam>
	public class AutomationHosting<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
	{
		private Task hostingTask;
		private CancellationTokenSource tokenSource;
		private ManualResetEvent mre;
		private MyInterface automation;
		private IdeaStatiCaClient<ClientInterface> bimClient;
		private Process bimProcess = null;
		private int processId;
		private readonly string EventName;
		private readonly string ClientUrlFormat;
		private readonly string AutomationUrlFormat;

#if DEBUG
		readonly TimeSpan OpenServerTimeLimit = TimeSpan.MaxValue;
#else
		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
#endif

		public AutomationHosting(MyInterface hostedService,
			string eventName = Constants.DefaultPluginEventName,
			string clientUrlFormat = Constants.DefaultPluginUrlFormat,
			string automationUrlFormat = Constants.DefaultIdeaStaticaAutoUrlFormat)
		{
			this.Status = AutomationStatus.Unknown;
			this.automation = hostedService;
			this.EventName = eventName;
			this.ClientUrlFormat = clientUrlFormat;
			this.AutomationUrlFormat = automationUrlFormat;
			mre = new ManualResetEvent(false);
		}

		public event ISEventHandler BIMStatusChanged;

		public AutomationStatus Status {get; private set;}

		public Task RunAsync(string id)
		{
			if (hostingTask != null)
			{
				Debug.Fail("Task is running");
				return Task.CompletedTask;
			}

			tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;

			HostingTask = Task.Run(() =>
			{
				RunServer(id, token);
			}, token);

			return HostingTask;
		}

		public ClientInterface MyBIM
		{
			get
			{
				if (bimClient == null || bimClient.State != CommunicationState.Opened)
				{
					return null;
				}

				return bimClient.Service;
			}
		}

		public void Stop()
		{
			if (hostingTask != null)
			{
				tokenSource.Cancel();
				var stopRes = mre.WaitOne();
				Debug.Assert(stopRes, "Can not stop");
			}
		}

		private void RunServer(string id, System.Threading.CancellationToken cancellationToken)
		{
			
			try
			{
				mre.Reset();
				try
				{
					processId = int.Parse(id);

					bimProcess = Process.GetProcessById(processId);
					bimProcess.EnableRaisingEvents = true;
					bimProcess.Exited += new EventHandler(BimProcess_Exited);

					// Connect to the pipe
					var feaPluginUrl = string.Format(ClientUrlFormat, id);

					NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

					bimClient = new IdeaStatiCaClient<ClientInterface>(pluginBinding, new EndpointAddress(feaPluginUrl));
					bimClient.Open();

					int counter = 0;
					while (bimClient.State != CommunicationState.Opened)
					{
						Thread.Sleep(100);
						if (counter > 200)
						{
							throw new CommunicationException("Can not open client");
						}
						counter++;
					}

					if (automation != null)
					{
						// service was injected
						if (automation is IClientBIM<ClientInterface> clientBIM)
						{
							clientBIM.BIM = bimClient.Service;
						}
					}

					Status |= AutomationStatus.IsClient;
				}
				catch (Exception)
				{
					bimProcess = null;
					processId = -1;
					if (automation != null)
					{
						// service was injected, set client's interface
						if (automation is IClientBIM<ClientInterface> clientBIM)
						{
							clientBIM.BIM = null;
						}
					}
				}

				string baseAddress = string.Format(AutomationUrlFormat, id);

				// expose my IAutomation interface
				using (ServiceHost selfServiceHost = new ServiceHost(automation, new Uri(baseAddress)))
				{
					((ServiceBehaviorAttribute)selfServiceHost.Description.
					Behaviors[typeof(ServiceBehaviorAttribute)]).InstanceContextMode
					= InstanceContextMode.Single;

					//Net named pipe
					NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647 };
					binding.ReceiveTimeout = TimeSpan.MaxValue;
					selfServiceHost.AddServiceEndpoint(typeof(IAutomation), binding, baseAddress);

					//MEX - Meta data exchange
					ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
					selfServiceHost.Description.Behaviors.Add(behavior);
					selfServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexNamedPipeBinding(), baseAddress + "/mex");

					selfServiceHost.Open(OpenServerTimeLimit);

					if (!string.IsNullOrEmpty(id))
					{
						// notify plugin that service is running
						string myEventName = string.Format("{0}{1}", EventName, id);
						EventWaitHandle syncEvent;
						if (EventWaitHandle.TryOpenExisting(myEventName, out syncEvent))
						{
							syncEvent.Set();
							syncEvent.Dispose();
						}
					}

#if DEBUG
					foreach (var endpoint in selfServiceHost.Description.Endpoints)
					{
						Debug.WriteLine("{0} ({1})", endpoint.Address.ToString(), endpoint.Binding.Name);
					}
#endif

					NotifyBIMStatusChanged(AppStatus.Started);

					while (!cancellationToken.IsCancellationRequested)
					{
						Thread.Sleep(100);
					}

					try
					{
						if (bimClient != null)
						{
							bimClient.Close();
							bimClient = null;
						}
					}
					catch { }

					try
					{
						if (selfServiceHost != null)
						{
							selfServiceHost.Close();
						}
					}
					catch { }

					NotifyBIMStatusChanged(AppStatus.Finished);

					mre.Set();
				}
			}
			catch (Exception e)
			{
				Debug.Assert(false, e.Message);
			}
		}

		protected void NotifyBIMStatusChanged(AppStatus newStatus)
		{
			BIMStatusChanged?.Invoke(this, new ISEventArgs() { Status = newStatus });
		}

		private void BimProcess_Exited(object sender, EventArgs e)
		{
			bimProcess.Exited -= new EventHandler(BimProcess_Exited);
			Status &= ~AutomationStatus.IsClient;
			bimProcess.Dispose();
			bimProcess = null;
			processId = -1;

			Stop();
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		public Task HostingTask { get => hostingTask; set => hostingTask = value; }
		public MyInterface Service { get => automation; }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (hostingTask != null)
					{
						try
						{
							Stop();
						}
						catch { }

						try
						{
							IDisposable disp = Service as IDisposable;
							if (disp != null)
							{
								disp.Dispose();
							}
							//feaAppService.Dispose();
							//feaAppService = null;
						}
						catch { }

						if (bimProcess != null)
						{
							bimProcess.Dispose();
							bimProcess = null;
						}

						mre.Dispose();
						tokenSource.Dispose();
					}
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~AutomationHosting() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}