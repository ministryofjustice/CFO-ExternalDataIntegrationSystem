using API.Endpoints.Actions;

namespace API.Endpoints
{
    public static class ReferenceEndpoints
    {
        /// <summary>
        /// Registers the reference endpoints, used modifying reference data
        /// </summary>
        public static IEndpointRouteBuilder RegisterReferenceEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/reference")
                .WithTags("Reference")
                .WithDisplayName("Reference endpoints");

            group.MapPost("{upci}/{orgCode}", SetStickyLocationAction.Action)
                .WithDescription(SetStickyLocationAction.Description)
                .Produces(StatusCodes.Status202Accepted)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization("write");

            group.RequireAuthorization("read");

            return routes;
        }
    }
}
