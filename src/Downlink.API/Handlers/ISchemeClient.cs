using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    /// <summary>
    /// This interface is intended as an extension point to support storage backends that return unsupported URI schemes.
    /// </summary>
    public interface ISchemeClient {
        IEnumerable<string> Schemes {get;}
        
        Task<IActionResult> GetContentAsync(IFileSource file);
    }
}
