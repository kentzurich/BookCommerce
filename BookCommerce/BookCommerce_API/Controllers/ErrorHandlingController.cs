using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookCommerce_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersionNeutral]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorHandlingController : ControllerBase
    {
        [Route("ProcessError")]
        public IActionResult ProcessError([FromServices] IHostEnvironment hostEnvironment)
        {
            if(hostEnvironment.IsDevelopment())
            {
                var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

                return Problem
                    (
                        detail: (feature.Error.InnerException != null ? feature.Error.InnerException.ToString() : feature.Error.StackTrace),
                        title: feature.Error.Message,
                        instance: hostEnvironment.EnvironmentName
                    );
            }
            else
            {
                return Problem();
            }
        }
    }
}
