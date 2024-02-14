using CItyInfo.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CItyInfo.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/files")]
    public class FileController : Controller
    {
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            
            var bytes = System.IO.File.ReadAllBytes("C:\\Users\\talyt\\Downloads\\teste.pdf");
            return File(bytes, "application/pdf", Path.GetFileName("C:\\Users\\talyt\\Downloads\\teste.pdf"));
        }
    }
}
