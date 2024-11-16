using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Entities;

namespace Photovoltaique.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        [HttpGet]
        public List<Site> GetSites()
        {
            return Data.Sites;
        }
    }
}
