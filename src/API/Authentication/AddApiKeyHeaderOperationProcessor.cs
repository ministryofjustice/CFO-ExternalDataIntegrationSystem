namespace API.Authentication;

public class AddApiKeyHeaderOperationProcessor : NSwag.Generation.Processors.IOperationProcessor
{
    public bool Process(NSwag.Generation.Processors.Contexts.OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Security ??= new List<NSwag.OpenApiSecurityRequirement>();
        context.OperationDescription.Operation.Security.Add(new NSwag.OpenApiSecurityRequirement
        {
            { "ApiKey", new List<string>() }
        });

        return true;
    }
}