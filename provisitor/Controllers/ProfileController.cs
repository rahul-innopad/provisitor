using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace provisitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProfileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("badge")]
        public IActionResult GetBadge()
        {
            try
            {
                var jsonDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Jsons", "visitor.json");
                var sourceJson = System.IO.File.ReadAllText(jsonDirectory);
                var srcObj = JsonDocument.Parse(sourceJson);
                var destObj = srcObj.RootElement;
                var json = JsonSerializer.Serialize(destObj);
                var obj = JsonSerializer.Deserialize<Visitor>(json);
    

                string svg = $@"
                    <svg xmlns=""http://www.w3.org/2000/svg"" width=""150"" height=""20"" role=""img"" aria-label=""Profile Views"">
                    <rect width=""150"" height=""20"" fill=""#555""/>
                    <rect x=""100"" width=""50"" height=""20"" fill=""#4c1""/>
                    <text x=""10"" y=""14"" font-family=""Verdana, Geneva, sans-serif"" font-size=""11"" fill=""#fff"">Profile Views</text>
                    <text x=""110"" y=""14"" font-family=""Verdana, Geneva, sans-serif"" font-size=""11"" fill=""#fff"">{obj?.visits}</text>
                    </svg>";

                Response.Headers.Append("Content-Type", "image/svg+xml");
                Response.Headers.Append("Cache-Control", "no-cache");
                return Content(svg, "image/svg+xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("track")]
        public IActionResult Tracker()
        {
            try
            {
                var jsonDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Jsons", "visitor.json");
                var sourceJson = System.IO.File.ReadAllText(jsonDirectory);
                var srcObj = JsonDocument.Parse(sourceJson);
                var destObj = srcObj.RootElement;
                var json = JsonSerializer.Serialize(destObj);
                var obj = JsonSerializer.Deserialize<Visitor>(json);

                obj.visits += 1;
                obj?.history?.Add(new History
                {
                    timestamp = DateTime.Now,
                    userAgent = Request.Headers["User-Agent"].ToString().Length,
                    referrer = Request.Headers["Referer"].ToString(),
                    ip = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                });

                // Serialize the updated obj instance to JSON
                var updatedJson = JsonSerializer.Serialize(obj);
                System.IO.File.WriteAllText(jsonDirectory, updatedJson);

                string svg = $@"
                    <svg xmlns=""http://www.w3.org/2000/svg"" width=""150"" height=""20"" role=""img"" aria-label=""Profile Views"">
                    <rect width=""150"" height=""20"" fill=""#555""/>
                    <rect x=""100"" width=""50"" height=""20"" fill=""#4c1""/>
                    <text x=""10"" y=""14"" font-family=""Verdana, Geneva, sans-serif"" font-size=""11"" fill=""#fff"">Profile Views</text>
                    <text x=""110"" y=""14"" font-family=""Verdana, Geneva, sans-serif"" font-size=""11"" fill=""#fff"">{obj?.visits}</text>
                    </svg>";

                Response.Headers.Append("Content-Type", "image/svg+xml");
                Response.Headers.Append("Cache-Control", "no-cache");
                return Content(svg, "image/svg+xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class Visitor
        {
            public int visits { get; set; }
            public List<History> history { get; set; }
        }

        public class History
        {
            public DateTime timestamp { get; set; }
            public int userAgent { get; set; }
            public string referrer { get; set; }
            public string ip { get; set; }
        }
    }
}
