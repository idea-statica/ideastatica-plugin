﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.gRPC.Reflection
{
    /// <summary>
    /// Invokes a method over gRPC.
    /// </summary>
    public class GrpcReflectionInvokeData
    {
        /// <summary>
        /// Nethod name to invoke.
        /// </summary>
        public string MethodName { get; set; } 

        /// <summary>
        /// Method parameters.
        /// </summary>
        public IEnumerable<GrpcReflectionArgument> Parameters { get; set; }
    } 
}
