//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.ServiceModel;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace IdeaStatiCa.Plugin.gRPC
//{
//    /// <summary>
//    /// gRPC Hosting implementation of <see cref="AutomationHosting{MyInterface, ClientInterface}"/>
//    /// </summary>
//    public class AutomationHostingRPC<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
//    {
//        private Task hostingTask;
//        private CancellationTokenSource tokenSource;
//        private ManualResetEvent mre;
//        private MyInterface automation; 
//        private Process bimProcess = null;
//        private int myAutomatingProcessId;
//        private readonly string EventName;
//        private readonly string ClientUrlFormat;
//        private readonly string AutomationUrlFormat; 
//        private IdeaStatiCaClient grpcClient;
//        private readonly Diagnostics.IIdeaLogger ideaLogger = null;

//        /// <summary>
//        /// My BIM object.
//        /// </summary>
//        public ClientInterface MyBIM
//        {
//            get
//            {
//                if (grpcClient == null || grpcClient.IsConnected)
//                {
//                    return null;
//                }

//                return bimClient.Service;
//            }
//        }

//        /// <summary>
//        /// Port on which the gRPC server is running.
//        /// </summary>
//        public int GrpcPort { get; private set; }

//        /// <summary>
//        /// Current automation status.
//        /// </summary>
//        public AutomationStatus Status { get; private set; }

//        /// <summary>
//        /// Triggered when BIM status changes.
//        /// </summary>
//        public event ISEventHandler BIMStatusChanged;

//#if DEBUG
//        private readonly TimeSpan OpenServerTimeLimit = TimeSpan.MaxValue;
//#else
//		private readonly TimeSpan OpenServerTimeLimit = TimeSpan.FromMinutes(1);
//#endif

//        public AutomationHostingRPC(MyInterface hostedService,
//            int grpcPort,
//            string eventName = Constants.DefaultPluginEventName)
//        {
//            ideaLogger = Diagnostics.IdeaDiagnostics.GetLogger("ideastatica.plugin.automationhostinggrpc");
            
//            Status = AutomationStatus.Unknown;
//            automation = hostedService;
//            EventName = eventName;            
//            mre = new ManualResetEvent(false);
//        }

//        /// <summary>
//        /// Starts the <see cref="AutomationHostingRPC{MyInterface, ClientInterface}".
//        /// </summary>
//        /// <param name="id">Client id</param>
//        /// <returns></returns>
//        public Task RunAsync(string id)
//        {
//            if (hostingTask != null)
//            {
//                Debug.Fail("Task is running");
//                return Task.CompletedTask;
//            }

//            tokenSource = new CancellationTokenSource();
//            var token = tokenSource.Token;

//            hostingTask = Task.Run(() =>
//            {
//                try
//                {
//                    RunServer(id, token);
//                }
//                catch (Exception e)
//                {
//                    ideaLogger.LogError("RunAsync  RunServer failed", e);
//                    throw;
//                }
//            }, token);

//            return hostingTask;
//        }

//        public void Stop()
//        {
//            if (hostingTask != null)
//            {
//                tokenSource.Cancel();
//                var stopRes = mre.WaitOne();

//                Debug.Assert(stopRes, "Can not stop");            }
//        }

//        protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
//        {
//            ideaLogger.LogInformation("Calling RunServer");

//            mre.Reset();

//            bool isBimRunning = false;

//            if (!string.IsNullOrEmpty(id))
//            {

//            }
//        }

//        #region IDisposable
//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
