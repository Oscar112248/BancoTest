using Banco.Application.Exceptions;
using Banco.Domain.Entities;
using Banco.Domain.Exceptions;
using Banco.Domain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Application.UseCases
{
    public sealed record CrearClienteCommand(
    string Nombre, char? Genero, int? Edad,
    string Identificacion, string? Direccion, string? Telefono,
    string CodigoCliente, string ContraseniaHash
);

    public sealed record GetClienteById(long clienteId);
    public sealed record GetClientes();

    public sealed record ActualizarClienteCommand(
       int Edad,
       string direccion,
       string telefono,
       long IdCliente

   );
    public sealed record ActualizarClienteSoftCommand(

      string IdCliente

  );


    public sealed class GetClientesHandler
    {
        private readonly IClienteRepository _repo;

        public GetClientesHandler(IClienteRepository repo) => _repo = repo;

        public async Task<Result<List<ClienteDomain?>>> HandleAsync(GetClientes query, CancellationToken ct)
        {

            var clientes = await _repo.ListAsync(ct);

            return Result<List<ClienteDomain?>>.Success(clientes, "Clientes consultados correctamente");
        }
    }
    public sealed class GetClienteByIdNumeroHandler
    {
        private readonly IClienteRepository _cliente;

        public GetClienteByIdNumeroHandler(IClienteRepository cliente) => _cliente = cliente;

        public async Task<Result<ClienteDomain?>> HandleAsync(GetClienteById query, CancellationToken ct)
        {
            var cliente = await _cliente.GetByPersonaIdAsync(query.clienteId, ct);
            if (cliente is null)
                return Result<ClienteDomain?>.Failure("Cliente inhabilitado o no existe");
            else
                return Result<ClienteDomain?>.Success(cliente, "Datos consultados correctamente¡");

        }

    }

    public sealed class CrearClienteHandler
    {
        private readonly IClienteRepository _repo;

        public CrearClienteHandler(IClienteRepository repo) => _repo = repo;

        public async Task<Result<long>> HandleAsync(CrearClienteCommand cmd, CancellationToken ct)
        {
            if (await _repo.ExisteIdentificacionAsync(cmd.Identificacion, ct))
                throw new ClienteExisteException(cmd.Identificacion);

            if (await _repo.ExisteCodigoClienteAsync(cmd.CodigoCliente, ct))
                throw new CodigoExisteException(cmd.CodigoCliente);


            var persona = new PersonaDomain(
                personaId: 0, cmd.Nombre, cmd.Genero, cmd.Edad,
                cmd.Identificacion, cmd.Direccion, cmd.Telefono, eliminado: false);

            var cliente = new ClienteDomain(
                personaId: 0, cmd.Nombre, cmd.Genero, cmd.Edad, cmd.Identificacion,
                cmd.Direccion, cmd.Telefono, eliminadoPersona: false,
                codigoCliente: cmd.CodigoCliente, contraseniaHash: cmd.ContraseniaHash,
                estado: false);

            var respuesta = await _repo.CrearPersonaYClienteAsync(persona, cliente, ct);
            if (respuesta != 0)
                return Result<long>.Success(respuesta, "Cliente creado correctamnente");
            else
                return Result<long>.Success("Error al crear cliente");

        }


        public sealed class ActualizarClienteHandler
        {
            private readonly IClienteRepository _repo;

            public ActualizarClienteHandler(IClienteRepository repo) => _repo = repo;

            public async Task<Result<bool>> HandleAsync(ActualizarClienteCommand cmd, CancellationToken ct)
            {
                var cliente = await _repo.GetByPersonaIdAsync(cmd.IdCliente, ct);
                if (cliente is null)
                    return Result<bool>.Failure("Cliente no encontrado");

                cliente.CambiarEdad(cmd.Edad);
                cliente.CambiarDireccion(cmd.direccion);
                cliente.CambiarTelefono(cmd.telefono);


                await _repo.UpdateAsync(cliente, ct);
                return Result<bool>.Success(true, "Cliente N° " + cmd.IdCliente + "  actualizado correctamente");
            }
        }

    }

    public sealed class ActualizarClienteSoftHandler
    {
        private readonly IClienteRepository _repo;

        public ActualizarClienteSoftHandler(IClienteRepository repo) => _repo = repo;

        public async Task<Result<bool>> HandleAsync(ActualizarClienteSoftCommand cmd, CancellationToken ct)
        {
            await _repo.SoftDeleteAsync(cmd.IdCliente, ct);
            return Result<bool>.Success(true, "Cliente N° " + cmd.IdCliente + "  eliminado correctamente");
        }
    }


}
