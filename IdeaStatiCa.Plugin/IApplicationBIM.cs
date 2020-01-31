﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public enum RequestedItemsType
	{
		Connections,
		Substructure
	}

	/// <summary>
	/// Abstraction of a FE application which provides data to Idea StatiCa
	/// </summary>
	[ServiceContract]
	public interface IApplicationBIM
	{
		[OperationContract]
		List<BIMItemId> GetActiveSelection();

		[OperationContract]
		string GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);

		[OperationContract]
		string GetApplicationName();

		[OperationContract]
		string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);

		[OperationContract]
		bool IsCAD();

		[OperationContract]
		Task SelectAsync(List<BIMItemId> items);
	}
}