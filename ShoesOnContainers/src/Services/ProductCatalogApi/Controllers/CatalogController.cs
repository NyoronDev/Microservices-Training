using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using System.Threading.Tasks;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext catalogContext;
        private readonly IOptionsSnapshot<CatalogSettings> settings;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings)
        {
            this.catalogContext = catalogContext;
            this.settings = settings;

            // No tracking property is used if you don´t need to change the database (read-only) for faster execution
            catalogContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await catalogContext.CatalogTypes.ToListAsync();

            return Ok(items);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await catalogContext.CatalogBrands.ToListAsync();

            return Ok(items);
        }
    }
}