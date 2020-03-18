using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Niverobot.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class EchoController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World!");
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]string request)
        {
            return Ok(request);
        }

        // PUT api/<controller>/5

    }
}
