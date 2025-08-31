using Banco.Application.UseCases;
using Banco.Infrastructure.Persistence.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using System.Reflection.Metadata;
using static Banco.Domain.Enums.CuentaEnum;

namespace BancoTest.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentaController : ControllerBase
    {
        private readonly CrearCuentaHandler _crearCuenta;
        private readonly GetCuentaByNumeroHandler _getByNumero;
        private readonly ActualizarCuentaHandler _actualizaCuenta;
        private readonly ActualizarEstadoCuentaHandler _actualizaCuentaEstado;
        private readonly GetCuentasHandler _getCuentas;

        public CuentaController(CrearCuentaHandler crearCuenta, GetCuentaByNumeroHandler getCuentaByNumeroHandler, ActualizarCuentaHandler actualizaCuenta, ActualizarEstadoCuentaHandler actualizaCuentaEstado,
            GetCuentasHandler getCuentasHandler)
        {
            _crearCuenta = crearCuenta;
            _getByNumero = getCuentaByNumeroHandler;
            _actualizaCuenta = actualizaCuenta;
            _actualizaCuentaEstado = actualizaCuentaEstado;
            _getCuentas = getCuentasHandler;    
        }

        [HttpGet("ConsultaCuentas")]
        public async Task<IActionResult> ConsultaCuentas(CancellationToken ct)
        {
            var resultado = await _getCuentas.HandleAsync(new GetCuentas(), ct);
            if (resultado is null) return NotFound();

            return Ok(resultado);

        }


        [HttpGet("ConsultaCuenta/{numeroCuenta}")]
        public async Task<IActionResult> ConsultaCuenta(string numeroCuenta, CancellationToken ct)
        {
            var resultado = await _getByNumero.HandleAsync(new GetCuentaByNumeroQuery(numeroCuenta), ct);
            if (resultado is null) return NotFound();

            return Ok(resultado);

        }


        [HttpPost("CrearCuenta")]
        public async Task<IActionResult> CrearCuenta([FromBody] CrearCuentaDto cuentaDto, CancellationToken ct)
        {
            if (!Enum.TryParse<TipoCuenta>(cuentaDto.TipoCuenta, ignoreCase: true, out var tipoCuenta))
                return BadRequest("TipoCuenta debe ser 'Ahorro' o 'Corriente'.");

            var command = new CrearCuentaCommand(
                ClienteIdPersona: cuentaDto.ClienteIdPersona,
                NumeroCuenta: cuentaDto.NumeroCuenta,
                TipoCuenta: tipoCuenta,
                SaldoInicial: cuentaDto.SaldoInicial
            );
            var cuentaId = await _crearCuenta.HandleAsync(command, ct);

            return Ok(cuentaId);

        }

        [HttpPut("ActualizaCuenta/{numeroCuenta}")]
        public async Task<IActionResult> PutCuenta(string numeroCuenta, [FromBody] ActualizarCuentaDto dto, CancellationToken ct)
        {
            if (!Enum.TryParse<TipoCuenta>(dto.TipoCuenta, true, out var tipo))
                return BadRequest("TipoCuenta debe ser 'Ahorro' o 'Corriente'.");

            var result = await _actualizaCuenta.HandleAsync(
                new ActualizarCuentaCommand(
                    NumeroCuenta: numeroCuenta,
                    TipoCuenta: tipo,
                    Estado: dto.Estado,
                    EliminaCuenta: dto.EliminaCuenta,
                    ClienteIdPersona: dto.ClienteIdPersona),
                ct);

            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result);
        }


        [HttpPatch("ActualizaEstadoCuenta/{numero}")]
        public async Task<IActionResult> PatchEstado([FromRoute] string numero,
            [FromBody] ActualizarEstadoCuentaDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest("El número de cuenta es requerido.");

            var cmd = new ActualizarEstadoCuentaCommand(numero, dto.Estado);
            var result = await _actualizaCuentaEstado.HandleAsync(cmd, true, ct);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
