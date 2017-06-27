using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public class RedirectingResponseHandler : ResponseHandler
    {
        public RedirectingResponseHandler(IEnumerable<ISchemeClient> additionalClients) : base(additionalClients)
        {
        }

        public override Task<IActionResult> HandleAsync(IFileSource file)
        {
            IActionResult result;
            switch (file.FileUri.Scheme)
            {
                case "file":
                    result =  new FileStreamResult(System.IO.File.OpenRead(file.FileUri.AbsolutePath), "application/octet-stream");
                    break;
                case "http":
                case "https":
                    result =  new RedirectResult(file.FileUri.AbsoluteUri);
                    break;
                default:
                    result = new StatusCodeResult(412);
                    break;
            }
            return Task.FromResult(result);
        }
    }
}
