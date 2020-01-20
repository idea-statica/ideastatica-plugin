using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public sealed class Constants
	{
		public const string AutomationParam = "-automation";
		public const string ProjectParam = "-project";
		public const string LibraryReposPath = "-libReposPath";

		public const string ConnectionChangedEventFormat = "IdeaStatiCaConnectionChanged{0}";
		public const string MemberChangedEventFormat = "IdeaStatiCaMemberChanged{0}";
		public const string ConCalculatorChangedEventFormat = "IdeaStatiCa.ConnHiddenCalculator-{0}";

		#region BIM Plugin default constants
		public const string DefaultPluginEventName = "IdeaStatiCaBIMPluginEvent";
		public const string DefaultPluginUrlFormat = "net.pipe://localhost/IdeaBIMPlugin{0}";
		public const string DefaultIdeaStaticaAutoUrlFormat = "net.pipe://localhost/IdeaStatiCaAuto{0}";
		#endregion

		#region Member plugin constants
		public const string MemberEventName = "MemberPluginEvent";
		public const string MemberUrlFormat = "net.pipe://localhost/IdeaMember{0}";
		#endregion

		public const string ConnHiddenCalculatorUrlFormat = "net.pipe://localhost/IdeaStatiCa.ConnHiddenCalculator{0}";
	}
}
