
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

        public IResponseHandler Handler { get; }
        private IConfiguration Configuration { get; }

        public DownlinkController(
            IConfiguration config,
            IHostingEnvironment env,
            MediatR.IMediator mediator,
            IResponseHandler handler,
            ILogger<DownlinkController> logger) : base(mediator)
        {
            Configuration = config;
            Handler = handler;
            _logger = logger;
            _environment = env;
        }

        [HttpGet]
        [Route("info")]
        public IActionResult GetInfo([FromServices] IEnumerable<IRemoteStorage> storage)
        {
            if (_environment.IsDevelopment())
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
        //[DownlinkRoute("{version}/{platform?}/{arch?}")]
        [Route("{version}/{platform?}/{arch?}")]
        public async Task<IActionResult> GetDownloadAsync(string version, string platform, string arch, [FromQuery] string format)
        {
            var spec = new VersionSpec(version, platform, arch);
            var req = new AppVersionRequest(spec, format);
            try
            {
                var res = await Mediator.Send(req);
                var result = await Handler.HandleAsync(res.Source);
                _logger?.LogInformation(101, result.ToString());
                return result;
            }
            catch (Core.Diagnostics.NotFoundException ex)
            {
                _logger?.LogWarning(404, ex, "Caught NotFoundException");
                return NotFound(ex.Message);
            }
        }
    }
}