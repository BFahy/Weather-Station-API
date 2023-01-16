using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WeatherStationAPI.Data.Repository;

namespace WeatherStationAPI.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class APIKeyAttribute : Attribute, IAsyncActionFilter
    {

        private string requiredRole;

        public string RequiredRole
        {
            get { return requiredRole; }
        }
                
        public APIKeyAttribute(string role = "Admin")
        {
            requiredRole = role;
        }


        /// <summary>
        /// Authentication attribute check
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.Request.Query.TryGetValue("APIKey", out var extractedKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key was not provided"
                };
                return;
            }

            var trimmedKey = extractedKey.ToString().Trim('{','}');
            var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();
            
            if (userRepo.AuthenticateUser(trimmedKey, RequiredRole) == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "User does not exist or has insufficient access"
                };
                return;
            }

            await next();
        }
    }
}
