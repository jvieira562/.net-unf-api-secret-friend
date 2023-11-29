using Microsoft.AspNetCore.Mvc;

using AmigoSecreto.API.Models;
using AmigoSecreto.API.ViewModels;
using AmigoSecreto.API.Services.Interfaces;

namespace AmigoSecreto.API.Controllers
{
    public class AmigoController : ControllerBase
    {
        #region [ Constructors ]
        private readonly IAmigoService _service;
        private readonly IParService _parService;

        public AmigoController(IAmigoService amigoService, IParService parService)
        {
            _service = amigoService;
            _parService = parService;
        }
        #endregion [ Constructors ]

        #region [ GET ]
        [HttpGet("/v1/buscar-todos")]
        public IActionResult BuscarTodosOsAmigos()
            => Ok(_service.GetAll());

        [HttpGet("/v1/email-existe/{email}")]
        public IActionResult EmailExiste([FromRoute] string email)
            => Ok(_service.GetAll().Any(amigo => amigo.Email == email));

        [HttpGet("v1/buscar-amigo/{id}")]
        public IActionResult GetAmigoById([FromRoute] string id)
        {
            var amigo = _service.GetById(id);
            return amigo is not null ?
            Ok(amigo) : NotFound($"Amigo com identificação {id} não foi encontrado.");
        }

        [HttpGet("/v1/buscar-pares")]
        public IActionResult BuscarPares()
            => Ok(_parService.GetAll());

        [HttpGet("/v1/buscar-par/{id}")]
        public IActionResult GetParById([FromRoute] string id)
        {
            if (id is null)
                return BadRequest("A identificação informada é inválida.");

            try
            {
                var dupla = _parService.GetById(Guid.Parse(id));
                return Ok(dupla);
            }
            catch
            {
                return BadRequest($"PX52R - A identificação informada é inválida {id}.\nRequest ID: " + Request.Headers.RequestId);
            }
        }

        #endregion [ GET ]

        #region [ POST ]

        [HttpPost("/v1/registrar")]
        public IActionResult SalvarAmigo([FromBody] CreateAmigoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Erro ao criar amigo" + model);

            var amigo = new Amigo(model.Name, model.Email);
            var result = _service.Save(amigo);

            if (!result)
                return BadRequest("ACS1 - Erro ao salvar registro.");

            return Created($"/v1/buscar-amigo/{amigo.Id}", result);
        }

        [HttpPost("/v1/gerar-pares")]
        public IActionResult GerarPares([FromBody] string flag)
        {
            if ((flag is null) || (flag != Configuration.GetFlag()))
                return Unauthorized("Contate um administrador.");

            var result = _parService.GerarPares();

            if (!result)
                return BadRequest("Número de participantes insuficientes ou impar.");

            return Ok(result);
        }

        #endregion [ POST ]

        #region [ DELETE ]
        [HttpDelete("/v1/excluir/{id}")]
        public IActionResult Excluir([FromRoute] string id)
        {
            if (id is null)
                return BadRequest($"5PX3 - Identificador inválido.");

            var idGuid = Guid.Parse(id);
            _service.Delete(idGuid);
            return NoContent();
        }
        #endregion [ DELETE ]

        #region [ PUT ]
        [HttpPut("/v1/atualizar")]
        public IActionResult Atualizar([FromBody] UpdateAmigoViewModel amigo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Os dados enviados são inválidos.");

            _service.Update(amigo.ToEntity());

            return NoContent();
        }
        #endregion [ PUT ]
    }
}