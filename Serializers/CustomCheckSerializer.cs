using Afterpelago.Models;
using Afterpelago.Services;
using BlazorWorker.WorkerBackgroundService;
using MudBlazor;
using Serialize.Linq.Serializers;
using System.Linq.Expressions;

namespace Afterpelago.Serializers
{
    public class CustomCheckSerializer : IExpressionSerializer
    {
        private readonly ExpressionSerializer serializer;

        public CustomCheckSerializer()
        { 
            var specificSerializer = new JsonSerializer();
            specificSerializer.AddKnownType(typeof(LogEntry));
            specificSerializer.AddKnownType(typeof(LogEntry[])); 
            specificSerializer.AddKnownType(typeof(CheckObtainedLogEntry));
            specificSerializer.AddKnownType(typeof(CheckObtainedLogEntry[]));
            specificSerializer.AddKnownType(typeof(Check));
            specificSerializer.AddKnownType(typeof(Check[]));
            specificSerializer.AddKnownType(typeof(ChartBuilderWebWorkerService));

            this.serializer = new ExpressionSerializer(specificSerializer); 
        } 

        public Expression Deserialize(string exprString)
        {
            return serializer.DeserializeText(exprString);
        }

        public string Serialize(Expression expr)
        {
            return serializer.SerializeText(expr);
        }
    }
}
