using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public interface IConnCalculatorFactory
	{
		ConnectionHiddenCheckClient Create();
	}

	public class ConnHiddenClientFactory : IDisposable
	{
		private readonly string IdeaInstallDir;
		Process CalculatorProcess { get; set; }
		Uri CalculatorUrl { get; set; }

#if DEBUG
		int StartTimeout = -1;
#else
		int StartTimeout = 1000*20;
#endif

		public ConnHiddenClientFactory(string ideaInstallDir)
		{
			IdeaInstallDir = ideaInstallDir;
		}

		public ConnectionHiddenCheckClient Create()
		{
			RunCalculatorProcess();

			NetNamedPipeBinding pluginBinding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

			ConnectionHiddenCheckClient calculatorClient = new ConnectionHiddenCheckClient(pluginBinding, new EndpointAddress(CalculatorUrl));
			calculatorClient.Open();

			return calculatorClient;
		}

		private void RunCalculatorProcess()
		{
			if (CalculatorProcess != null)
			{
				return;
			}

			int myProcessId = Process.GetCurrentProcess().Id;

			string eventName = string.Format(Constants.ConCalculatorChangedEventFormat, myProcessId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				string connChangedEventName = string.Format(Constants.ConCalculatorChangedEventFormat, myProcessId);
				string applicationExePath = Path.Combine(IdeaInstallDir, "IdeaStatiCa.ConnHiddenCalculator.exe");

				string cmdParams = $"-automation{myProcessId}";
				ProcessStartInfo psi = new ProcessStartInfo(applicationExePath, cmdParams);
				psi.WindowStyle = ProcessWindowStyle.Normal;
				psi.UseShellExecute = false;

#if DEBUG
				psi.CreateNoWindow = false;
#else
				psi.CreateNoWindow = true;
#endif

				CalculatorProcess = new Process();
				CalculatorProcess.StartInfo = psi;
				CalculatorProcess.EnableRaisingEvents = true;
				CalculatorProcess.Start();

				if (!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException();
				}
			}

			CalculatorUrl = new Uri(string.Format(Constants.ConnHiddenCalculatorUrlFormat, CalculatorProcess.Id));
			CalculatorProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (CalculatorProcess == null)
			{
				return;
			}

			CalculatorProcess.Dispose();
			CalculatorProcess = null;
			CalculatorUrl = null;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					try
					{
						if (CalculatorProcess != null)
						{
							CalculatorProcess.EnableRaisingEvents = false;
							CalculatorProcess.Kill();
							CalculatorProcess = null;
							CalculatorUrl = null;
						}
					}
					catch
					{

					}
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ConnHiddenClientFactory()
		// {
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
		#endregion
	}
}
