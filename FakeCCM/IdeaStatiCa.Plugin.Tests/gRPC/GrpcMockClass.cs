using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
    public enum GrpcMockEnum
    {
        Value1,
        Value2
    }

    public class GrpcMockSubClass
    {
        public string Title { get; set; }

        public int ID { get; set; }

        public GrpcMockSubClass Child { get; set; }
    }

    public class GrpcMockClass
    {
        public string Value { get; set; }

        public async Task<string> TestMethod(
            string param1, 
            int param2, 
            short param3, 
            bool param4, 
            GrpcMockEnum param5, 
            GrpcMockSubClass param6)
        {
            await Task.Delay(10);

            return Value;
        }
    }
}
