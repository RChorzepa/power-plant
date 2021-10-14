using Microsoft.AspNetCore.Mvc;
using PowerPlant.Core.Repositories;
using PowerPlant.Infrastructure.Services.ProductionPagedServices;
using System.Threading.Tasks;

namespace PowerPlant.WebUI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionPagedService _productionPagedService;
        private readonly IProductionRepositoryAsync _productionRepositoryAsync;

        public ProductionController(IProductionPagedService productionPagedService, IProductionRepositoryAsync productionRepositoryAsync)
        {
            _productionPagedService = productionPagedService;
            _productionRepositoryAsync = productionRepositoryAsync;
        }

        public ActionResult GetPaged(string filter, int page = 1, int pageSize = 20)
        {
            var data = _productionPagedService.GetPaged(filter, page, pageSize);

            return Ok(data);
        }

        [HttpGet("criterias")]
        public ActionResult GetCriterias()
        {
            var criterias = _productionPagedService.GetFilterCriteraValues();

            return Ok(criterias);
        }

        [HttpGet("generate")]
        public  ActionResult GenerateFakeData(int year, int generators)
        {
            if (year == 0 || generators == 0)
                return BadRequest();

            Task.Run(() => _productionRepositoryAsync.GenerateFakeDataAsync(year, generators));

            return Ok();
        }
    }
}
