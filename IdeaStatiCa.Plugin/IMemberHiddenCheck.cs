using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Provides seamlessly data about connections or allows to run hidden calculation of a connection
	/// </summary>
	[ServiceContract]
	public interface IMemberHiddenCheck
	{
		/// <summary>
		/// Open idea connection project in the hidden calculator
		/// </summary>
		/// <param name="projectLocation">File name of the ideacon project file</param>
		[OperationContract]
		void OpenProject(string projectLocation);

		[OperationContract]
		string Calculate(int subStructureId);

		/// <summary>
		/// Close project which is open in the service
		/// </summary>
		[OperationContract]
		void CloseProject();

		/// <summary>
		/// Cancel current calcullation
		/// </summary>
		[OperationContract]
		void Cancel();
	}
}