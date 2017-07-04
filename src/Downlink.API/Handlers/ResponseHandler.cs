using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public abstract class ResponseHandler : IResponseHandler
    {
        protected ResponseHandler(IEnumerable<ISchemeClient> additionalClients) {
            AdditionalClients = additionalClients;
        }

        protected IEnumerable<ISchemeClient> AdditionalClients { get; }

        public abstract Task<IActionResult> HandleAsync(IFileSource file);

        protected async Task<IActionResult> HandleUnknownAsync(IFileSource file) {
            var client = AdditionalClients.FirstOrDefault(c => c.Schemes.Contains(file.FileUri.Scheme));
            if (client == null) {
                return new StatusCodeResult(412);
            }
            return await client.GetContentAsync(file);
        }
    }
}
