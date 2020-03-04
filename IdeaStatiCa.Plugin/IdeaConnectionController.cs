﻿using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public class IdeaConnectionController : IDisposable
	{
		private readonly string IdeaInstallDir;
		private Process IdeaStatiCaProcess { get; set; }
		private Uri CalculatorUrl { get; set; }

		protected EventWaitHandle CurrentItemChangedEvent;

		protected IdeaStatiCaClient<IAutomation> ConnectionAppClient { get; set; }
		protected virtual uint UserMode { get; } = 0;

		public IAutomation ConnectionAppAutomation
		{
			get
			{
				return ConnectionAppClient?.Service;
			}
		}

		private string BaseAddress { get; set; }

#if DEBUG
		private int StartTimeout = -1;
#else
		int StartTimeout = 1000*20;
#endif

		private IdeaConnectionController(string ideaInstallDir)
		{
			IdeaInstallDir = ideaInstallDir;
		}

		public static IdeaConnectionController Create(string ideaInstallDir)
		{
			IdeaConnectionController connectionController = new IdeaConnectionController(ideaInstallDir);
			connectionController.OpenConnectionClient();
			return connectionController;
		}

		private void OpenConnectionClient()
		{
			int processId = Process.GetCurrentProcess().Id;
			string connChangedEventName = string.Format(Constants.ConnectionChangedEventFormat, processId);
			CurrentItemChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, connChangedEventName);

			string applicationExePath = Path.Combine(IdeaInstallDir, "ideaconnection.exe");

			Process connectionProc = new Process();
			string eventName = string.Format("IdeaStatiCaEvent{0}", processId);
			using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
			{
				connectionProc.StartInfo = new ProcessStartInfo(applicationExePath, $"-cmd:automation-{processId}  user-mode 192");
				connectionProc.EnableRaisingEvents = true;
				connectionProc.Start();

				if(!syncEvent.WaitOne(StartTimeout))
				{
					throw new TimeoutException();
				}
			}

			IdeaStatiCaProcess = connectionProc;

			BaseAddress = $"net.pipe://localhost/ConnectioService{connectionProc.Id}";

			NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };
			ConnectionAppClient = new IdeaStatiCaClient<IAutomation>(binding, new EndpointAddress(BaseAddress));
			ConnectionAppClient.Open();

			IdeaStatiCaProcess.Exited += CalculatorProcess_Exited;
		}

		private void CalculatorProcess_Exited(object sender, EventArgs e)
		{
			if (IdeaStatiCaProcess == null)
			{
				return;
			}

			IdeaStatiCaProcess.Dispose();
			IdeaStatiCaProcess = null;
			CalculatorUrl = null;
			ConnectionAppClient = null;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ConnectionControllerFactory()
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
