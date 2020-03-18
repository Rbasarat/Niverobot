using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;


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
        public IActionResult Post([FromBody] Update body)
        {
            return Ok("you did it:" + body.Message.Text);

        }
    }
}
