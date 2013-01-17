HybridApiActionSelector
=======================

HTTP verb and action name hybrid dispatcher for Web API

Usage:

1. Reference the Strathweb.HybridApiActionSelector project (or dll)

2. Replace the default action selector in your API with the new one:
   config.Services.Replace(typeof(IHttpActionSelector), new HybridActionSelector());

You can read the introduction post on the blog:
http://www.strathweb.com/2013/01/magical-web-api-action-selector-http-verb-and-action-name-dispatching-in-a-single-controller/

The code itself is adapted from the Web API core DefaultApiActionSelector (from System.Web.Http)
https://github.com/ASP-NET-MVC/aspnetwebstack/blob/master/src/System.Web.Http/Controllers/ApiControllerActionSelector.cs
