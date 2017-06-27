using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public class ProxyingResponseHandler : ResponseHandler
    {
        public ProxyingResponseHandler(IEnumerable<ISchemeClient> clients) : base(clients)
        {
        }

        public override async Task<IActionResult> HandleAsync(IFileSource file)
        {
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
