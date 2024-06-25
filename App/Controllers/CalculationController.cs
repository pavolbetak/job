using App.Data;
using Microsoft.AspNetCore.Mvc;
using App.MediaTypes;
using App.Services;
using App.Services.Dto;

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
        [Consumes(MediaTypeNames.Text.Json)]
        public void CalculateData([FromRoute] int key, [FromBody]CalculationData data)
        {
            var item = _storageService.SaveDataIntoStorage(new ItemDataDto
            {
                Key = key,
                Value = data.Input,
            });
        }
    }
}
