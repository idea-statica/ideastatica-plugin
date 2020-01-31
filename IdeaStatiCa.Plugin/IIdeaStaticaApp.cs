using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Identification of an item in IDEA StatiCa MPRL (material and product range library)
	/// </summary>
	[DataContract]
	public class LibraryItem
	{
		[DataMember]
		public string Group { get; set; }

		[DataMember]
		public string Identifier { get; set; }

		[DataMember]
		public string Name { get; set; }
	}

	/// <summary>
	/// Identification of an item in the Idea project
	/// </summary>
	[DataContract]
	public class ProjectItem
	{
		/// <summary>
		/// Id of the item in the idea project
		/// </summary>
		[DataMember]
		public int Identifier { get; set; }

		/// <summary>
		/// Name of the item in the idea project
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}

	[ServiceContract]
	public interface IIdeaStaticaApp
	{
		/// <summary>
		/// Get all cross-sections from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Cross-sections in the MPRL</returns>
		[OperationContract]
		List<LibraryItem> GetCssInMPRL(IdeaRS.OpenModel.CountryCode countryCode);

		/// <summary>
		/// Get all materials from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Materials in the MPRL</returns>
		[OperationContract]
		List<LibraryItem> GetMaterialsInMPRL(IdeaRS.OpenModel.CountryCode countryCode);

		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections in the project</returns>
		[OperationContract]
		List<ProjectItem> GetCssInProject();

		/// <summary>
		/// Get all materials in the currently open project
		/// </summary>
		/// <returns>Materials in the project</returns>
		[OperationContract]
		List<ProjectItem> GetMaterialsInProject();
	}
}
