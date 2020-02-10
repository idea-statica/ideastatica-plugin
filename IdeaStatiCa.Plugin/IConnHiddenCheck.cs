using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Provides seamlessly data about connections or allows to run hidden calculation of a connection
	/// </summary>
	[ServiceContract]
	public interface IConnHiddenCheck
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project.</param>
		[OperationContract]
		void OpenProject(string ideaConProject);

		/// <summary>
		/// Gets details about the open project and its connections
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		ConProjectInfo GetProjectInfo();

		/// <summary>
		/// Cakculate connection given by <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The identifier of the required connection</param>
		/// <returns>Connection check results</returns>
		[OperationContract]
		ConnectionResultsData Calculate(string connectionId);

		/// <summary>
		/// Get the geometry of the <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the required connection</param>
		/// <returns>Geometry of the connection in the IOM format</returns>
		[OperationContract]
		IdeaRS.OpenModel.Connection.ConnectionData GetConnectionModel(string connectionId);

		[OperationContract]
		/// <summary>
		/// Creates Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/> and projects saves into the <paramref name="newIdeaConFileName"/>
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		/// <param name="newIdeaConFileName">File name of idea connection project where generated project will be saved</param>
		void CreateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName, string newIdeaConFileName);

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
