using Banco.Application.Exceptions;
using Banco.Domain.Entities;
using Banco.Domain.Exceptions;
using Banco.Domain.Interfaces;
using Banco.Domain.Services;
using Shared;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Application.UseCases
{
    public sealed class RealizarDepositoHandler
    {
        private readonly ICuentaRepository _cuentas;
        private readonly IMovimientoRepository _movs;
        private readonly MovimientoDomainService _domSvc;

        public RealizarDepositoHandler(
            ICuentaRepository cuentas,
            IMovimientoRepository movs,
            MovimientoDomainService domSvc)
        {
            _cuentas = cuentas;
            _movs = movs;
            _domSvc = domSvc;
        }

        public async Task<Result<MovimientoCreadoResult>> HandleAsync(RealizarDepositoCommand cmd, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByNumeroAsync(cmd.NumeroCuenta, false, ct)
                         ?? throw new CuentaInactivaException(cmd.NumeroCuenta);


            var mov = _domSvc.Depositar(cuenta, cmd.Valor, cmd.Fecha);

            await _movs.InsertAsync(mov, ct);

            var saldo = await _cuentas.ObtenerSaldoActualAsync(cuenta.CuentaId, ct);

            var payload = new MovimientoCreadoResult(
                mov.MovimientoId,
                cmd.NumeroCuenta,
                cmd.Valor,

                mov.Fecha,
                saldo);

            return Result<MovimientoCreadoResult>.Success(payload, "Depósito registrado");
        }
    }

    public sealed record GetMovimientos();

    public sealed class RealizarRetiroHandler
    {
        private readonly ICuentaRepository _cuentas;
        private readonly IMovimientoRepository _movs;
        private readonly MovimientoDomainService _domSvc;

        public RealizarRetiroHandler(
            ICuentaRepository cuentas,
            IMovimientoRepository movs,
            MovimientoDomainService domSvc)
        {
            _cuentas = cuentas;
            _movs = movs;
            _domSvc = domSvc;

        }

        public async Task<Result<MovimientoCreadoResult>> HandleAsync(RealizarRetiroCommand cmd, CancellationToken ct)
        {
            var cuenta = await _cuentas.GetByNumeroAsync(cmd.NumeroCuenta, true, ct)
                         ?? throw new CuentaExisteException(cmd.NumeroCuenta);

            var (ini, fin) = VentanaLocalDia.CalculaDia((DateTime)cmd.Fecha);

            var saldoActual = await _cuentas.ObtenerSaldoActualAsync(cuenta.CuentaId, ct);
            var retirosDia = await _movs.ObtenerRetirosNoAnuladosAsync(cuenta.CuentaId, ini, fin, ct);

            var mov = await _domSvc.Retirar(cuenta, cmd.Valor, saldoActual, retirosDia, cmd.Fecha);

            await _movs.InsertAsync(mov, ct);

            var saldoFinal = await _cuentas.ObtenerSaldoActualAsync(cuenta.CuentaId, ct);

            var payload = new MovimientoCreadoResult(
                mov.MovimientoId,
                cmd.NumeroCuenta,
                cmd.Valor,
               mov.Fecha,
                saldoFinal);

            return Result<MovimientoCreadoResult>.Success(payload, "Retiro registrado");
        }
    }

    public sealed class GetMovimientosHandler
    {
        private readonly IMovimientoRepository _repo;

        public GetMovimientosHandler(IMovimientoRepository repo) => _repo = repo;

        public async Task<Result<List<MovimientoCuentasDto?>>> HandleAsync(GetMovimientos query, CancellationToken ct)
        {

            var movimientos = await _repo.ObtenerMovimientos(ct);

            return Result<List<MovimientoCuentasDto?>>.Success(movimientos.ToList(), "Movimientos consultados correctamente");
        }
    }


    public static class VentanaLocalDia
    {
        public static (DateTime inicio, DateTime fin) CalculaDia(DateTime referencia)
        {
            var start = new DateTime(referencia.Year, referencia.Month, referencia.Day, 0, 0, 0);
            var end = start.AddDays(1);
            return (start, end);
        }
    }
}
