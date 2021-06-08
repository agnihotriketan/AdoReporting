using AdoReporting.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AdoReporting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [EnableCors("SpecificOrigin")]
    public class AdoDataController : ControllerBase
    {
        private readonly ILogger<AdoDataController> _logger;
        private readonly IQueryExecutor _queryExecutor;

        public AdoDataController(ILogger<AdoDataController> logger, IQueryExecutor queryExecutor)
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Get(string projectName)
        {
            try
            {
                var data = _queryExecutor.GetWorkItemHierarchy(projectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult UpdateWorkItem(int id, string title)
        {
            try
            {
                bool status = _queryExecutor.UpdateWorkItem(id, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return Ok();
        }
    }
}
