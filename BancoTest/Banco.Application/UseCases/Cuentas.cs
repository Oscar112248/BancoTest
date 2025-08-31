using Banco.Application.Exceptions;
using Banco.Domain.Entities;
using Banco.Domain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Application.UseCases
{
    public sealed record CrearCuentaCommand(
     long ClienteIdPersona,
     string NumeroCuenta,
     TipoCuenta TipoCuenta,
     decimal SaldoInicial
 );
    public sealed record ActualizarCuentaCommand(
        string NumeroCuenta,
        TipoCuenta TipoCuenta,
        bool Estado,
        bool EliminaCuenta,
        long ClienteIdPersona
    );
    public sealed record ActualizarEstadoCuentaCommand(
        string NumeroCuenta,
        bool Estado
    );
    public sealed record GetCuentaByNumeroQuery(string NumeroCuenta);
    public sealed record GetCuentas();



    public sealed class CrearCuentaHandler
    {
        private readonly ICuentaRepository _cuentas;

        public CrearCuentaHandler(ICuentaRepository cuentas) => _cuentas = cuentas;

        public async Task<Result<long>> HandleAsync(CrearCuentaCommand cmd, CancellationToken ct)
        {
            if (!await _cuentas.NumeroDisponibleAsync(cmd.NumeroCuenta, ct))
                throw new CuentaExisteException(cmd.NumeroCuenta);

            var cuenta = new CuentaDomain(
                0, cmd.ClienteIdPersona, cmd.NumeroCuenta, cmd.TipoCuenta,
                cmd.SaldoInicial, estado: false, fechaAperturaUtc: DateTime.UtcNow);

            var id = await _cuentas.CrearAsync(cuenta, ct);
            return Result<long>.Success("Cuenta  N° " + cuenta.NumeroCuenta + " creada");
        }
    }

    public sealed class GetCuentaByNumeroHandler
    {
        private readonly ICuentaRepository _cuentas;

        public GetCuentaByNumeroHandler(ICuentaRepository cuentas) => _cuentas = cuentas;

        public async Task<Result<CuentaDomain?>> HandleAsync(GetCuentaByNumeroQuery query, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByNumeroAsync(query.NumeroCuenta, false, ct);
            if (cuenta is null)
                return Result<CuentaDomain?>.Failure("Cuenta inhabilitado o no existe");
            else
                return Result<CuentaDomain?>.Success(cuenta, "Datos consultados correctamente¡");

        }
    }

    public sealed class ActualizarCuentaHandler
    {
        private readonly ICuentaRepository _repo;

        public ActualizarCuentaHandler(ICuentaRepository repo) => _repo = repo;

        public async Task<Result<bool>> HandleAsync(ActualizarCuentaCommand cmd, CancellationToken ct)
        {
            var cuenta = await _repo.GetByNumeroAsync(cmd.NumeroCuenta, false, ct);
            if (cuenta is null)
                return Result<bool>.Failure("Cuenta no encontrada");

            cuenta.CambiarTipoCuenta(cmd.TipoCuenta);
            if (cmd.Estado) cuenta.Activar(); else cuenta.Desactivar();


            await _repo.UpdateAsync(cuenta, ct);
            return Result<bool>.Success(true, "Cuenta N° " + cmd.NumeroCuenta + "  actualizada correctamente");
        }
    }

    public sealed class ActualizarEstadoCuentaHandler
    {
        private readonly ICuentaRepository _repo;

        public ActualizarEstadoCuentaHandler(ICuentaRepository repo) => _repo = repo;

        public async Task<Result<bool>> HandleAsync(ActualizarEstadoCuentaCommand cmd, bool solicita, CancellationToken ct)
        {
            var cuenta = await _repo.GetByNumeroAsync(cmd.NumeroCuenta, solicita, ct);
            if (cuenta is null)
                return Result<bool>.Failure("Cuenta no encontrada");

            if (cmd.Estado)
                cuenta.Activar();
            else
                cuenta.Desactivar();

            await _repo.UpdateAsync(cuenta, ct);

            return Result<bool>.Success(true,
                $"Estado de la cuenta N° {cmd.NumeroCuenta} actualizado correctamente");
        }
    }

    public sealed class GetCuentasHandler
    {
        private readonly ICuentaRepository _repo;

        public GetCuentasHandler(ICuentaRepository repo) => _repo = repo;

        public async Task<Result<List<CuentaDomain?>>> HandleAsync(GetCuentas query, CancellationToken ct)
        {

            var cuentas = await _repo.ListAsync(ct);

            return Result<List<CuentaDomain?>>.Success(cuentas, "Clientes consultados correctamente");
        }
    }
}
