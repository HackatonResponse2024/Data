using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Controllers.Dto.Down;
using Photovoltaique.API.Entities;

namespace Photovoltaique.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        [HttpGet]
        public List<SiteDown> GetSites()
        {
            return Data.Sites.Select(site => new SiteDown(site)).ToList();
        }
    }
}
