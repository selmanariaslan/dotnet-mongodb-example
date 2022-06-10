using Arch.CoreLibrary.Entities;
using Arch.CoreLibrary.Repositories;
using Arch.Data;
//using Arch.Mongo.Managers;
//using Arch.Mongo.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Arch.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly CoreLibrary.Managers.IServiceManager _Service;
        protected readonly IMemoryCache _Cache;
        protected readonly IGenericRepository<LogManagementContext> _LogService;

        public BaseApiController(CoreLibrary.Managers.IServiceManager service, IMemoryCache cache, IGenericRepository<LogManagementContext> logService)
        {
            _Service = service;
            _Cache = cache;
            _LogService = logService;
        }

        protected async Task<IActionResult> Api<T>(ResponseBase<T> response, bool controlData = true)
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
                    return StatusCode(409, response);
                }
            }
        }
    }
}
