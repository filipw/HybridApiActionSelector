using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Strathweb.HybridApiActionSelector
{
    public class HybridActionSelector : ApiControllerActionSelector
    {
        private readonly IDictionary<ReflectedHttpActionDescriptor, string[]> _actionParams = new Dictionary<ReflectedHttpActionDescriptor, string[]>();
        
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            object actionName, subactionName;
            var hasActionName = controllerContext.RouteData.Values.TryGetValue("action", out actionName);
            var hasSubActionName = controllerContext.RouteData.Values.TryGetValue("subaction", out subactionName); 

            var method = controllerContext.Request.Method;
            var allMethods = controllerContext.ControllerDescriptor.ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var validMethods = Array.FindAll(allMethods, IsValidActionMethod);

            var actionDescriptors = new HashSet<ReflectedHttpActionDescriptor>();

            foreach (var actionDescriptor in validMethods.Select(m => new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, m)))
            {
                actionDescriptors.Add(actionDescriptor);

                _actionParams.Add(
                    actionDescriptor,
                    actionDescriptor.ActionBinding.ParameterBindings
                                 .Where(b => !b.Descriptor.IsOptional && b.Descriptor.ParameterType.UnderlyingSystemType.IsPrimitive )
                                 .Select(b => b.Descriptor.Prefix ?? b.Descriptor.ParameterName).ToArray());
            }

            IEnumerable<ReflectedHttpActionDescriptor> actionsFoundSoFar;

            if (hasSubActionName)
            {
                actionsFoundSoFar =
                    actionDescriptors.Where(
                        i => i.ActionName.ToLowerInvariant() == subactionName.ToString().ToLowerInvariant() && i.SupportedHttpMethods.Contains(method)).ToArray();
            }
            else if (hasActionName)
            {
                actionsFoundSoFar =
                    actionDescriptors.Where(
                        i =>
                        i.ActionName.ToLowerInvariant() == actionName.ToString().ToLowerInvariant() &&
                        i.SupportedHttpMethods.Contains(method)).ToArray();
            }
            else
            {
                actionsFoundSoFar = actionDescriptors.Where(i => i.ActionName.ToLowerInvariant().Contains(method.ToString().ToLowerInvariant()) && i.SupportedHttpMethods.Contains(method)).ToArray();   
            }

            var actionsFound = FindActionUsingRouteAndQueryParameters(controllerContext, actionsFoundSoFar);

            if (actionsFound == null || !actionsFound.Any()) throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cannot find a matching action."));
            if (actionsFound.Count() > 1) throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.Ambiguous, "Multiple matches found."));

            return actionsFound.FirstOrDefault();
        }

        private IEnumerable<ReflectedHttpActionDescriptor> FindActionUsingRouteAndQueryParameters(HttpControllerContext controllerContext, IEnumerable<ReflectedHttpActionDescriptor> actionsFound)
        {
            var routeParameterNames = new HashSet<string>(controllerContext.RouteData.Values.Keys, StringComparer.OrdinalIgnoreCase);

            if (routeParameterNames.Contains("controller")) routeParameterNames.Remove("controller");
            if (routeParameterNames.Contains("action")) routeParameterNames.Remove("action");
            if (routeParameterNames.Contains("subaction")) routeParameterNames.Remove("subaction");

            var hasQueryParameters = controllerContext.Request.RequestUri != null && !String.IsNullOrEmpty(controllerContext.Request.RequestUri.Query);
            var hasRouteParameters = routeParameterNames.Count != 0;

            if (hasRouteParameters || hasQueryParameters)
            {
                var combinedParameterNames = new HashSet<string>(routeParameterNames, StringComparer.OrdinalIgnoreCase);
                if (hasQueryParameters)
                {
                    foreach (var queryNameValuePair in controllerContext.Request.GetQueryNameValuePairs())
                    {
                        combinedParameterNames.Add(queryNameValuePair.Key);
                    }
                }

                actionsFound = actionsFound.Where(descriptor => _actionParams[descriptor].All(combinedParameterNames.Contains));

                if (actionsFound.Count() > 1)
                {
                    actionsFound = actionsFound
                        .GroupBy(descriptor => _actionParams[descriptor].Length)
                        .OrderByDescending(g => g.Key)
                        .First();
                }
            }
            else
            {
                // return actions with no parameters
                actionsFound = actionsFound.Where(descriptor => _actionParams[descriptor].Length == 0);
            }

            return actionsFound;
        }

        private static bool IsValidActionMethod(MethodInfo methodInfo)
        {
            if (methodInfo.IsSpecialName) return false;
            return !methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(ApiController));
        }
    }
}