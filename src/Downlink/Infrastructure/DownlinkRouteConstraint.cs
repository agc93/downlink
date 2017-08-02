using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        internal DownlinkActionConstraint(string prefix, ILogger logger)
        {
            Prefix = prefix;
            _logger = logger;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            _logger.LogDebug("Matching route using prefix: {0}", Prefix ?? string.Empty);
            return string.IsNullOrWhiteSpace(Prefix)
                //? !context.RouteContext.RouteData.Values.Any(v => v.Key == "prefix" && !string.IsNullOrWhiteSpace(v.Value.ToString()))
                ? true
                : context.RouteContext.RouteData.Values.First().Value?.ToString() == Prefix;
        }
    }

    public class DownlinkRouteConvention : IActionModelConvention
    {
        private readonly string _prefix;
        private readonly ILoggerFactory _factory;
        //private readonly AttributeRouteModel _routeModel;
        private readonly ILogger _logger;

        public DownlinkRouteConvention(IRoutePrefixBuilder routeBuilder, ILoggerFactory factory)
        {
            _prefix = routeBuilder.GetPrefix();
            _factory = factory;
            _logger = factory.CreateLogger(nameof(DownlinkRouteConvention));
        }
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerType == typeof(Controllers.DownlinkController))
            {
                if (string.IsNullOrWhiteSpace(_prefix))
                {
                    _logger.LogInformation("No route prefix defined! Skipping route merging.");
                }
                else
                {
                    var routeModel = new AttributeRouteModel(new RouteAttribute(_prefix));
                    //this is slightly dangerous logic.
                    // we should be separately iterating over selectors based on their AttributeRouteModel value
                    // but a) that's hard and b) we already know all the actions have attribute routes
                    foreach (var selector in action.Selectors)
                    {
                        _logger.LogInformation("Route prefix found! Merging '{0}' into Downlink routes.", _prefix);
                        /*selector.ActionConstraints.Add(
                            new DownlinkActionConstraint(
                                _prefix,
                                _factory.CreateLogger(nameof(DownlinkActionConstraint)))); */
                        selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            routeModel,
                            selector.AttributeRouteModel
                        );
                    }
                }
            }
        }
    }
}