using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Downlink.Controllers
{
    public class ApiController : Controller
    {
        protected IMediator Mediator {get;}

        protected ApiController(IMediator mediator) {
            Mediator = mediator;
        }

        protected IActionResult Gone() {
            return new GoneResult();
        }

        protected class GoneResult : IActionResult
        {
            public Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Gone;
                return Task.CompletedTask;
            }
        }
    }
}