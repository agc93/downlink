using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Handlers
{
    public interface IExceptionHandler
    {
        Task<IActionResult> HandleAsync(NotFoundException exception, VersionSpec version);
    }

    public class ExceptionHandler : IExceptionHandler
    {
        public Task<IActionResult> HandleAsync(NotFoundException exception, VersionSpec version)
        {
            return Task.FromResult(new NotFoundObjectResult(exception.Message) as IActionResult);
        }
    }
}