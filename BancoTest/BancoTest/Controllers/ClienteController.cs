using Banco.Application.Exceptions;
using Banco.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using static Banco.Application.UseCases.CrearClienteHandler;
using static Banco.Domain.Enums.CuentaEnum;

namespace BancoTest.Controllers
{

    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {

        private readonly CrearClienteHandler _crearCliente;
        private readonly GetClienteByIdNumeroHandler _getClienteByIdNumero;
        private readonly ActualizarClienteHandler _actualizaCliente;
        private readonly GetClientesHandler _getClientes;
        private readonly ActualizarClienteSoftHandler _actualizaSoft;





        public ClienteController(CrearClienteHandler crearClienteHandler, GetClienteByIdNumeroHandler getClienteByIdNumeroHandler, ActualizarClienteHandler actualizarClienteHandler,
            GetClientesHandler getClientesHandler, ActualizarClienteSoftHandler actualizarClienteSoftHandler)
        {
            _crearCliente = crearClienteHandler;
            _getClienteByIdNumero = getClienteByIdNumeroHandler;
            _actualizaCliente = actualizarClienteHandler;
            _getClientes = getClientesHandler;
            _actualizaSoft = actualizarClienteSoftHandler;

        }
        [HttpGet("ConsultaCliente/{clienteId}")]
        public async Task<IActionResult> ConsultaCuenta(long clienteId, CancellationToken ct)
        {
            var resultado = await _getClienteByIdNumero.HandleAsync(new GetClienteById(clienteId), ct);
            if (resultado is null) return NotFound();

            return Ok(resultado);

        }


        [HttpGet("ConsultaClientes")]
        public async Task<IActionResult> ConsultaClientes(CancellationToken ct)
        {
            var resultado = await _getClientes.HandleAsync(new GetClientes(), ct);
            if (resultado is null) return NotFound();

            return Ok(resultado);

        }

        [HttpPost]
        public async Task<IActionResult> CrearAsync([FromBody] CrearClienteDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);
            var command = new CrearClienteCommand(
               Nombre: dto.Nombre, Genero: dto.Genero, Edad: dto.Edad, Identificacion: dto.Identificacion
               , Direccion: dto.Direccion, Telefono: dto.Telefono, CodigoCliente: dto.CodigoCliente,
               ContraseniaHash: dto.ContraseniaHash

           );

            var cliente = await _crearCliente.HandleAsync(command, ct);
            return Ok(cliente);

        }


        [HttpPut("ActualizaCliente/{idCliente}")]
        public async Task<IActionResult> PutCliente(long idCliente, [FromBody] ActualizaClienteDto dto, CancellationToken ct)
        {
            var result = await _actualizaCliente.HandleAsync(
     new ActualizarClienteCommand(Edad: dto.Edad, direccion: dto.Direccion, telefono: dto.Telefono, IdCliente: idCliente), ct);


            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result);
        }

        [HttpPatch("EliminarCliente/{idCliente}")]
        public async Task<IActionResult> EliminarCliente([FromRoute] string idCliente,
            [FromBody] ActualizarEstadoCuentaDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(idCliente))
                return BadRequest("El idCliente de cliente es requerido.");

            var cmd = new ActualizarClienteSoftCommand(idCliente);
            var result = await _actualizaSoft.HandleAsync(cmd, ct);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

    }
}
