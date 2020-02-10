using System;
using System.Threading;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;

namespace IdeaStatiCa.Plugin
{
	public class ConnectionHiddenCheckClient : System.ServiceModel.ClientBase<IConnHiddenCheck> , IConnHiddenCheck
	{
		public static int HiddenCalculatorId { get; set; }

		static ConnectionHiddenCheckClient()
		{
			HiddenCalculatorId = -1;
		}

		public ConnectionHiddenCheckClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public ConnectionResultsData Calculate(string connectionId)
		{
			return Service.Calculate(connectionId);
		}

		public void CloseProject()
		{
			Service.CloseProject();
		}

		public ConProjectInfo GetProjectInfo()
		{
			return Service.GetProjectInfo();
		}

		public void OpenProject(string ideaConFileName)
		{
			Service.OpenProject(ideaConFileName);
		}

		public ConnectionData GetConnectionModel(string connectionId)
		{
			return Service.GetConnectionModel(connectionId);
		}
		public void CreateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName, string newIdeaConFileName)
		{
			Service.CreateConProjFromIOM(iomXmlFileName, iomResXmlFileName, newIdeaConFileName);
		}

		public void Cancel()
		{
			if(HiddenCalculatorId < 0)
			{
				return;
			}

			EventWaitHandle syncEvent;
			var cancelEventName = string.Format(Constants.ConCalculatorCancelEventFormat, HiddenCalculatorId);
			if (EventWaitHandle.TryOpenExisting(cancelEventName, out syncEvent))
			{
				syncEvent.Set();
				syncEvent.Dispose();
			}
		}

		protected IConnHiddenCheck Service => base.Channel;
	}
}
