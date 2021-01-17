using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.gRPC.Reflection
{
    /// <summary>
    /// Client for the <see cref="GrpcReflectionHandler"/> enabled <see cref="GrpcClient"/>
    /// </summary>
    public class GrpcReflectionClient
    {
        private GrpcSynchronousClient client;

        /// <summary>
        /// Determines whether the client is connected.
        /// </summary>
        public bool IsConnected => client.IsConnected;

        /// <summary>
        /// Initializes the <see cref="GrpcReflectionClient"/>
        /// </summary>
        /// <param name="clientId">ID of the client.</param>
        /// <param name="port">Port on which <see cref="GrpcServer"/> is running.</param>
        public GrpcReflectionClient(string clientId, int port)
        {
            client = new GrpcSynchronousClient(clientId, port);
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            await client.ConnectAsync();
        }

        /// <summary>
        /// Invokes remote method over gRPC via reflection.
        /// </summary>
        /// <typeparam name="T">Type to which the result will be converted.</typeparam>
        /// <param name="methodName">Name of the method to invoke.</param>
        /// <param name="arguments">Arguments with which the method will be called.</param>
        /// <returns></returns>
        public async Task<T> InvokeMethodAsync<T>(string methodName, params object[] arguments)
        {
            var parsedArgs = ReflectionHelper.GetMethodInvokeArguments(arguments);
            var request = new GrpcReflectionInvokeData()
            {
                MethodName = methodName,
                Parameters = parsedArgs
            };
            var data = JsonConvert.SerializeObject(request);
            var response = await client.SendMessageSync(GrpcReflectionHandler.GRPC_REFLECTION_HANDLER_MESSAGE, data);

            // hadnle response
            var responseData = JsonConvert.DeserializeObject<T>(response.Data);

            return responseData;
        }
    }
}
