
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Handlers;
using Downlink.Infrastructure;
using Downlink.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink.Controllers
{
    public class DownlinkController : ApiController
    {
        private readonly ILogger<DownlinkController> _logger;
        private readonly IHostingEnvironment _environment;

        public IExceptionHandler Exceptionhandler { get; }
        public IResponseHandler Handler { get; }
        private IConfiguration Configuration { get; }

        private List<string> IgnoredKeywords = new List<string> { "robots.txt", "favicon.ico", "sitemap.xml"};

        public DownlinkController(
            IConfiguration config,
            IHostingEnvironment env,
            MediatR.IMediator mediator,
            IResponseHandler handler,
            IExceptionHandler exHandler,
            ILogger<DownlinkController> logger) : base(mediator)
        {
            Configuration = config;
            Handler = handler;
            _logger = logger;
            _environment = env;
            Exceptionhandler = exHandler;
        }

        [HttpGet]
        [Route("info", Name = "DownlinkGetInfo")]
        public IActionResult GetInfo([FromServices] IEnumerable<IRemoteStorage> storage)
        {
            if (_environment.IsDevelopment() && Request.IsLocal())
            {
                return Ok(new
                {
                    version = this.GetType().Assembly.GetName().Version.ToString(),
                    config = Configuration.AsEnumerable().Select(k => $"{k.Key}={k.Value}").ToList(),
                    availableStorage = storage.Select(s => s.Name)
                });
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{version}/{platform?}/{arch?}", Name = "DownlinkGetDownload")]
        public async Task<IActionResult> GetDownloadAsync(string version, string platform, string arch, [FromQuery] string format)
        {
            if (IgnoredKeywords.Contains(version)) return NotFound();
            var spec = new VersionSpec(version, platform, arch);
            var req = new AppVersionRequest(spec, format);
            try
            {
                var res = await Mediator.Send(req);
                await Mediator.Publish(new DownlinkResultNotification(spec, res));
                var result = await Handler.HandleAsync(res.Source);
                _logger?.LogInformation(101, result.ToString());
                return result;
            }
            catch (Core.Diagnostics.NotFoundException ex)
            {
                _logger?.LogWarning(404, ex, "Caught NotFoundException");
                await Mediator.Publish(ex.ToNotification(spec));
                var result = await Exceptionhandler.HandleAsync(ex, spec);
                return result;
            }
        }
    }
}