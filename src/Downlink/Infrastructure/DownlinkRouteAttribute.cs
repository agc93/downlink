using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

namespace Downlink.Infrastructure
{
    internal class DownlinkRouteAttribute : Attribute, IActionConstraintFactory, IRouteTemplateProvider
    {
        internal static string _prefix;
        private string _template;
        internal DownlinkRouteAttribute(string template) {
            _template = template;
        }
        public string Template => $"{(_prefix ?? string.Empty)}{_template}";

        public int? Order {get;set;}

        public string Name {get;set;}

        public bool IsReusable => true;

        public IActionConstraint CreateInstance(IServiceProvider services)
        {
            throw new NotImplementedException();
        }
    }
}