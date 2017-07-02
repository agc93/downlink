using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Downlink.Handlers
{
    public class ProxyingResponseHandler : ResponseHandler
    {
        private readonly ILogger<ProxyingResponseHandler> _logger;

        public ProxyingResponseHandler(
            IEnumerable<ISchemeClient> clients,
            ILogger<ProxyingResponseHandler> logger
        ) : base(clients)
        {
            _logger = logger;
        }

        public override async Task<IActionResult> HandleAsync(IFileSource file)
        {
            _logger.LogInformation("Handling proxying response for source {0}", file.FileUri.ToString());
            switch (file.FileUri.Scheme)
            {
                case "file":
                    return new FileStreamResult(System.IO.File.OpenRead(file.FileUri.AbsolutePath), "application/octet-stream");
                default:
                    return await HandleUnknownAsync(file);
            }
        }
    }
}
