using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public abstract class SchemeClient : ISchemeClient
    {
        protected SchemeClient(IEnumerable<string> schemes)
        {
            Schemes = schemes;
        }

        protected SchemeClient(string scheme)
        {
            Schemes = new List<string> { scheme };
        }
        public IEnumerable<string> Schemes { get; }

        public abstract Task<IActionResult> GetContentAsync(IFileSource file);
    }
}