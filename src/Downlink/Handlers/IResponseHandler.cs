using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public interface IResponseHandler
    {
        Task<IActionResult> HandleAsync(IFileSource file);
    }
}
