﻿using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin
{
	public enum BIMItemType
	{
		Member,
		Node
	}

	[DataContract]
	public class BIMItemId
	{
		[DataMember]
		public BIMItemType Type { get; set; }

		[DataMember]
		public int Id { get; set; }
	}
}