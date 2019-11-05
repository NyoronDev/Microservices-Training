using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;

        public PictureController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetImage(int id)
        {
            var webRoot = environment.WebRootPath;
            var path = Path.Combine($"{webRoot}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}", $"shoes-{id}.png");
            var buffer = System.IO.File.ReadAllBytes(path);

            return File(buffer, "image/png");
        }
    }
}