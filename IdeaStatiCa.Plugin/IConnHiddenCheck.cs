using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using System;
using System.Collections.Generic;
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
		/// Creates Idea connection project from given <paramref name="openModel"/>, <paramref name="openModelResult"/> and projects saves into the <paramref name="newIdeaConFileName"/>
		/// </summary>
		/// <param name="openModel"></param>
		/// <param name="openModelResult"></param>
		/// <param name="newIdeaConFileName"></param>
		void CreateConProjFromIOM(OpenModel openModel, OpenModelResult openModelResult, string newIdeaConFileName);

		/// <summary>
		/// 
		/// </summary>
		[OperationContract]
		void CloseProject();
	}
}
