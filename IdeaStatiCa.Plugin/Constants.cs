﻿namespace IdeaStatiCa.Plugin
{
	public sealed class Constants
	{
		public const string CodeCheckManagerAppName = "IdeaCodeCheck.exe";
		public const string AutomationParam = "-automation";
		public const string ProjectParam = "-project";
		public const string LibraryReposPath = "-libReposPath";

		public const string ConnectionChangedEventFormat = "IdeaStatiCaConnectionChanged{0}";
		public const string MemberChangedEventFormat = "IdeaStatiCaMemberChanged{0}";
		public const string ConCalculatorChangedEventFormat = "IdeaStatiCa.ConnHiddenCalculator-{0}";
		public const string ConCalculatorCancelEventFormat = "IdeaStatiCa.ConnHiddenCalculatorCancel-{0}";
		public const string MemHiddenCalcChangedEventFormat = "IdeaStatiCa.MemberHiddenCalculator-{0}";
		public const string MemHiddenCalcCancelEventFormat = "IdeaStatiCa.MemberHiddenCalculatorCancel-{0}";

		#region BIM Plugin default constants

		public const string DefaultPluginEventName = "IdeaStatiCaBIMPluginEvent";
		public const string DefaultPluginUrlFormat = "net.pipe://localhost/IdeaBIMPlugin{0}";
		public const string DefaultIdeaStaticaAutoUrlFormat = "net.pipe://localhost/IdeaStatiCaAuto{0}";
		public const string ProgressCallbackUrlFormat = "net.pipe://localhost/IdeaStatiCaProgress{0}";

		#endregion BIM Plugin default constants

		#region Member plugin constants

		public const string MemberEventName = "MemberPluginEvent";
		public const string MemberUrlFormat = "net.pipe://localhost/IdeaMember{0}";

		#endregion Member plugin constants

		public const string ConnHiddenCalculatorUrlFormat = "net.pipe://localhost/IdeaStatiCa.ConnHiddenCalculator{0}";

		public const string MemberHiddenCalculatorUrlFormat = "net.pipe://localhost/IdeaStatiCa.MemberHiddenCalculator{0}";
	}
}