using AdoReporting.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AdoReporting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[EnableCors("SpecificOrigin")]
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
        [Route("{projectName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetAdoWorkItems(string projectName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(projectName))
                {
                    var data = _queryExecutor.GetWorkItemHierarchy(projectName).Result;
                    return Ok(data);
                }
                else
                    throw new ArgumentNullException();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("{id}/{title}")]
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
