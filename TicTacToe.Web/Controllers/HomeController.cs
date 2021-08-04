using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicTacToe.Core.Interfaces.Services;
using TicTacToe.Common.Responses;

namespace TicTacToe.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameService _gameService;

        public HomeController(ILogger<HomeController> logger,
            IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> History()
        {
            var games = await _gameService.GetGamesHistory();
            return View(games);
        }

        [HttpPost]
        public async Task<MoveResponse> Move([FromQuery]string userCoordinate)
        {
            string userId;

            if (!HttpContext.Request.Cookies.TryGetValue("userId", out userId))
            {
                userId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("userId", userId);
            }

            return await _gameService.Move(userCoordinate, userId);
        }
        
    }
}
