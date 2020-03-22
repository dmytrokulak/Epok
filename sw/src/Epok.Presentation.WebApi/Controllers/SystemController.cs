using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    /// <summary>
    /// Controller to retrieve system information.
    /// </summary>
    [Route("api/system")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// Can be used for response testing.
        /// </summary>
        [HttpGet("ping")]
        public void GetPing()
        {
            //ToDo:5 some health check info
        }
    }
}