namespace IdeaStatiCa.Plugin
{
	public class MemberHiddenCheckClient : System.ServiceModel.ClientBase<IMemberHiddenCheck>, IMemberHiddenCheck
	{
		public MemberHiddenCheckClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public void OpenProject(string projectLocation)
		{
			Service.OpenProject(projectLocation);
		}

		public void CloseProject()
		{
			Service.CloseProject();
		}

		public string Calculate(int subStructureId)
		{
			return Service.Calculate(subStructureId);
		}

		protected IMemberHiddenCheck Service => base.Channel;
	}
}
