using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieRank.Services;

namespace MovieRank.Controllers
{
    [Route("setup")]
    public class SetupController : Controller
    {
        private readonly ISetupService _setupService;

        public SetupController(ISetupService setupService)
        {
            _setupService = setupService;
        }
        /// <summary>
        /// create a dynamoDb table
        /// </summary>
        /// <param name="dynamoDbTableName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createTable/{dynamoDbTableName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDynamoDbTable(string dynamoDbTableName)
        {
            await _setupService.CreateDynamoDbTable(dynamoDbTableName);

            return Ok();
        }
        /// <summary>
        /// Delete a dynamoDb table
        /// </summary>
        /// <param name="dynamoDbTableName"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteTable/{dynamoDbTableName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTable(string dynamoDbTableName)
        {
            await _setupService.DeleteDynamoDbTable(dynamoDbTableName);

            return Ok();
        }
    }
}