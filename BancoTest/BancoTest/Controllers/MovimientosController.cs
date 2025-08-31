using Banco.Application.Exceptions;
using Banco.Application.UseCases;
using Banco.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;

namespace BancoTest.Controllers
{
    [ApiController]
    [Route("api/movimientos")]

    public class MovimientosController : ControllerBase
    {

        private readonly RealizarDepositoHandler _deposito;
        private readonly RealizarRetiroHandler _retiro;
        private readonly GetMovimientosHandler _listar;


        public MovimientosController(RealizarDepositoHandler realizarDepositoHandler , RealizarRetiroHandler realizarRetiroHandler
            , GetMovimientosHandler listar)
        {
            _deposito = realizarDepositoHandler;
            _retiro = realizarRetiroHandler;
            _listar = listar;
        }

        [HttpGet("ConsultaMovimientos")]
        public async Task<IActionResult> ConsultaMovimientos(CancellationToken ct)
        {
            var resultado = await _listar.HandleAsync(new GetMovimientos(), ct);
            if (resultado is null) return NotFound();

            return Ok(resultado);

        }

        [HttpPost("depositos")]
        public async Task<IActionResult> Depositar([FromBody] DepositoDto req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var resultado = await _deposito.HandleAsync(new RealizarDepositoCommand(req.NumeroCuenta, req.Valor, req.Fecha), ct);
            return Ok(resultado);

        }


        [HttpPost("retiros")]
        public async Task<IActionResult> Retirar([FromBody] RetiroDto req, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var res = await _retiro.HandleAsync(new RealizarRetiroCommand(req.NumeroCuenta, req.Valor, req.Fecha), ct);
            return Ok(res);


        }
    }
}
