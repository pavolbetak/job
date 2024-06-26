using App.Data;
using Microsoft.AspNetCore.Mvc;
using App.Services;

namespace App.Controllers
{
    [ApiController]
    public class CalculationController : ControllerBase
    {
        private readonly StorageService _storageService;

        public CalculationController(StorageService storageService) 
        {
            _storageService = storageService;
        }

        [HttpPost]
        [Route("[controller]/{key:int}")]
        [Consumes("text/json")]
        public CalculatedData CalculateData([FromRoute] int key, [FromBody]CalculationInputData data)
        {
            var calculatedData = _storageService.SaveDataIntoStorageAndNotify(key, data.Input);

            return calculatedData;
        }
    }
}
