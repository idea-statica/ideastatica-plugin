using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.gRPC.Reflection
{
    /// <summary>
    /// Contains response returned from gRPC service.
    /// </summary>
    public class GrpcReflectionCallbackData
    {
        /// <summary>
        /// FullName of the value type.
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Value returned from server.
        /// </summary>
        public object Value { get; set; }
    }
}
