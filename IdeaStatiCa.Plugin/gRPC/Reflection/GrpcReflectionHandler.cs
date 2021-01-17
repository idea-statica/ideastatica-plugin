﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.gRPC.Reflection
{
    /// <summary>
    /// Handler that invokes methods for specified object over gRPC via reflection.
    /// </summary>
    public class GrpcReflectionHandler : IGrpcMessageHandler<object>
    {
        public const string GRPC_REFLECTION_HANDLER_MESSAGE = "Grpc.Handlers.Reflection";

        private object instance;

        /// <summary>
        /// Initializes new <see cref="GrpcReflectionHandler"/>
        /// </summary>
        /// <param name="instance">Instance for which the messages will be handled.</param>
        public GrpcReflectionHandler(object instance)
        {
            if(instance == null)
            {
                throw new ArgumentException("Instance cannot be null.");
            }

            this.instance = instance;
        }

        public async Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server)
        {
            try
            {
                var grpcInvokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(message.Data);
                var arguments = grpcInvokeData.Parameters;
                var result = await ReflectionHelper.InvokeMethodFromGrpc(instance, grpcInvokeData.MethodName, arguments);
                var jsonResult = result != null ? JsonConvert.SerializeObject(result) : null;

                await server.SendMessageAsync(
                    message.OperationId,
                    message.MessageName,
                    jsonResult
                    );

                return result;
            }
            catch (Exception e)
            {
                await server.SendMessageAsync(message.OperationId, "Error", e.Message);

                return null;
            }
        }

        public Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client)
        {
            var callback = JsonConvert.DeserializeObject<GrpcReflectionCallbackData>(message.Data);
            var value = callback.Value != null ? JsonConvert.DeserializeObject(callback.Value.ToString(), Type.GetType(callback.ValueType)) : null;

            return Task<object>.FromResult(value);
        }
    }
}
