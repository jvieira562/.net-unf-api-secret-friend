using AmigoSecreto.Web.Models;
using AmigoSecreto.Web.ViewModels;

using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace AmigoSecreto.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiHelper _api;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _api = new();
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel homeModel = new();
            var result = await _api.GetAsync("/v1/buscar-todos");

            homeModel.Amigos = JsonConvert
                .DeserializeObject<List<Amigo>>(result)
                ?.OrderBy(obj => obj.RegistradoEm)
                .ToList() ?? new List<Amigo>();

            return View(homeModel);
        }
        [HttpGet("amigo-secreto")]
        public IActionResult AmigoSecreto()
        {
            return View();
        }
        [HttpGet("participantes")]
        public IActionResult Participantes()
        {
            return View("Participantes");
        }
        [Authorize]
        [HttpGet("meu-amigo-secreto")]
        public IActionResult Dupla()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> NovoAmigo(CreateAmigoViewModel model)
        {
            var homeModel = new HomeViewModel();

            if (!ModelState.IsValid)
            {
                homeModel.Feedback = FeedbackModel.Create(Tipo.Error, $"PX4E - Erro ao registrar {model.Name}. Tente novamente, se o problema persistir, contate um administrador.");
                return View("Index", homeModel);
            }

            var emailExiste = JsonConvert
                .DeserializeObject<bool>(
                await _api.GetAsync("/v1/email-existe/" + model?.Email?.Replace(";", "")));

            if (emailExiste)
            {
                homeModel.Feedback = FeedbackModel.Create(Tipo.Info, $"PX43 - Erro ao registrar {model?.Name}. O email informado já está em uso.");
                return View("Index", homeModel);
            }

            var result = await _api.PostAync("/v1/registrar", model);

            if (result)
            {
                homeModel.Feedback = FeedbackModel.Create(Tipo.Success, $"{model?.Name} foi cadastrado com sucesso.");
                return RedirectToAction("Index", homeModel);
            }
            else
            {
                homeModel.Feedback = FeedbackModel.Create(Tipo.Error, $"PX5E - Erro ao registrar {model.Name}. Tente novamente, se o problema persistir, contate um administrador.");
                return View("Index", homeModel);
            }
        }
    }
}