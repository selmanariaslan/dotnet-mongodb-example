using Arch.CoreLibrary.Entities;
using Arch.Mongo.Managers;
using Arch.Mongo.Models;
using Arch.Mongo.Models.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Arch.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IServiceManager _Service;
        protected readonly IMemoryCache _Cache;
        protected readonly IGenericRepository<PerformanceLog> _LogService;

        public BaseApiController(IServiceManager service, IMemoryCache cache, IGenericRepository<PerformanceLog> logService)
        {
            _Service = service;
            _Cache = cache;
            _LogService = logService;
        }

        protected IActionResult Api<T>(ResponseBase<T> response, bool controlData = true)
        {
            if (response.Status == ServiceResponseStatuses.Success)
            {
                return Ok(response);
            }
            else
            {
                if (controlData && EqualityComparer<T>.Default.Equals(response.Data, default))
                {
                    return NotFound(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
        }
    }
}
