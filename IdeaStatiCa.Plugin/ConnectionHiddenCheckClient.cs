using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public class ConnectionHiddenCheckClient : System.ServiceModel.ClientBase<IConnHiddenCheck>, IConnHiddenCheck
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

		public void SaveAsProject(string newProjectFileName)
		{
			Service.SaveAsProject(newProjectFileName);
		}

		public string ApplyTemplate(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting)
		{
			return Service.ApplyTemplate(connectionId, conTemplateFileName, connTemplateSetting);
		}

		public string ExportToTemplate(string connectionId, string conTemplateFileName)
		{
			return Service.ExportToTemplate(connectionId, conTemplateFileName);
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
			if (HiddenCalculatorId < 0)
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

		public List<ProjectItem> GetMaterialsInProject()
		{
			return Service.GetMaterialsInProject();
		}

		public List<ProjectItem> GetCrossSectionsInProject()
		{
			return Service.GetCrossSectionsInProject();
		}

		public List<ProjectItem> GetBoltAssembliesInProject()
		{
			return Service.GetBoltAssembliesInProject();
		}

		public int AddBoltAssembly(string boltAssemblyName)
		{
			return Service.AddBoltAssembly(boltAssemblyName);
		}

		protected IConnHiddenCheck Service => base.Channel;
	}
}