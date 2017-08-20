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

        public override async Task<IActionResult> HandleAsync(IFileSource file)
        {
            IActionResult result;
            switch (file.FileUri.Scheme)
            {
                case "http":
                case "https":
                    result = new RedirectResult(file.FileUri.AbsoluteUri);
                    break;
                default:
                    return await HandleUnknownAsync(file);
            }
            return result;
        }
    }
}
