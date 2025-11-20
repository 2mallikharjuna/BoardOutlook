using Microsoft.AspNetCore.Authorization;

namespace BoardOutlook.Api.Middleware
{
    /// <summary>
    /// Authorize Request Reader Requirment
    /// </summary>
    public class ApiReaderAuth : IAuthorizationRequirement
    {
        public ApiReaderAuth() { }
    }
    /// <summary>
    /// Artists Reader Requirment Handler
    /// </summary>
    public class ArtistsReaderRequirmentHandler : AuthorizationHandler<ApiReaderAuth>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiReaderAuth requirement)
        {
            context.Succeed(requirement); //Setting always true for Artists Reader
            return Task.FromResult<object>(null);
        }
    }
}
