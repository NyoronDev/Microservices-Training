using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModels;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await catalogContext.CatalogItems.SingleOrDefaultAsync(m => m.Id == id);

            if (!(item is null))
            {
                item.PictureUrl = item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", settings.Value.ExternalCatalogBaseUrl);
                return Ok(item);
            }

            return NotFound();
        }

        //GET api/Catalog/items[?pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await catalogContext.CatalogItems.LongCountAsync();
            var itemsOnPage = await catalogContext.CatalogItems.OrderBy(m => m.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        //GET api/Catalog/items/withName/Wonder[?pageSize=2&pageIndex=3]
        [HttpGet]
        [Route("[action]/withName/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await catalogContext.CatalogItems.Where(m => m.Name.StartsWith(name)).LongCountAsync();
            var itemsOnPage = await catalogContext.CatalogItems.Where(m => m.Name.StartsWith(name)).OrderBy(m => m.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        //GET api/Catalog/items/type/1/brand/1[?pageSize=2&pageIndex=3]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(int? catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 6, [FromQuery] int pageIndex = 0)
        {
            // With IQueryable we are not sending this to the database
            var root = (IQueryable<CatalogItem>)catalogContext.CatalogItems;

            if (catalogTypeId.HasValue)
            {
                root = root.Where(m => m.CatalogTypeId == catalogTypeId);
            }

            if (catalogBrandId.HasValue)
            {
                root = root.Where(m => m.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root.OrderBy(m => m.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            itemsOnPage = ChangeUrlPlaceHolder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalItems, itemsOnPage);
            return Ok(model);
        }

        private List<CatalogItem> ChangeUrlPlaceHolder(List<CatalogItem> items)
        {
            items.ForEach(m => m.PictureUrl = m.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", settings.Value.ExternalCatalogBaseUrl));

            return items;
        }
    }
}