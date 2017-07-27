using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink.Infrastructure
{
    public class DownlinkActionConstraint : IActionConstraint, IActionConstraintMetadata
    {
        private readonly ILogger _logger;

        public string Prefix { get; private set; }
        internal DownlinkActionConstraint(string prefix, ILogger logger) {
            Prefix = prefix;
            _logger = logger;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            _logger.LogDebug("Matching route using prefix: {0}", Prefix ?? string.Empty);
            return string.IsNullOrWhiteSpace(Prefix)
                ? true
                : context.RouteContext.RouteData.Values.First().Value?.ToString() == Prefix;
        }
    }

    public class DownlinkRouteConvention : IActionModelConvention
    {
        private readonly string _prefix;
        private readonly ILoggerFactory _factory;

        public DownlinkRouteConvention(IConfiguration config, ILoggerFactory factory) {
            _prefix = config.GetValue("DownlinkPrefix", string.Empty);
            _factory = factory;
        }
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerType == typeof(Controllers.DownlinkController)) {
                System.Console.WriteLine("Got a Downlink route");
                foreach(var selector in action.Selectors) {
                    selector.ActionConstraints.Add(
                        new DownlinkActionConstraint(
                            _prefix,
                            _factory.CreateLogger(nameof(DownlinkActionConstraint))));
                }
            }
        }
    }
}