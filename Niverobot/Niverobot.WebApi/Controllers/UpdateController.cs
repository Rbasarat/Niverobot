using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Niverobot.WebApi.Interfaces;
using Telegram.Bot.Types;

namespace Niverobot.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly ITelegramUpdateService _updateService;

        public UpdateController(ITelegramUpdateService updateService)
        {
            _updateService = updateService;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateService.EchoAsync(update);
            return Ok();
        }
    }
}
