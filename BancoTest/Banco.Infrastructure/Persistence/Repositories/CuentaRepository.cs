using Banco.Domain.Entities;
using Banco.Domain.Interfaces;
using Banco.Infrastructure.Exceptions;
using Banco.Infrastructure.Mapper;
using Banco.Infrastructure.Persistence.Context;
using Banco.Infrastructure.Persistence.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Infrastructure.Persistence.Repositories
{
    public sealed class CuentaRepository : ICuentaRepository
    {
        private readonly BANCO_DbContext _db;
        public CuentaRepository(BANCO_DbContext db) => _db = db;

        public async Task<CuentaDomain?> GetByNumeroAsync(string numeroCuenta, bool solicita, CancellationToken ct)
        {
            var ef = new Cuentum();
            if (solicita)
                ef = await _db.Cuenta
                     .SingleOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta, ct);
            else
                ef = await _db.Cuenta
                   .SingleOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta && !c.Estado, ct);


            return ef?.ToDomain();
        }

        public async Task<CuentaDomain?> GetByIdAsync(long cuentaId, CancellationToken ct)
        {
            var ef = await _db.Cuenta
                .SingleOrDefaultAsync(c => c.CuentaId == cuentaId && !c.Estado, ct);
            return ef?.ToDomain();
        }

        public async Task<long> CrearAsync(CuentaDomain cuenta, CancellationToken ct)
        {
            var ef = new Cuentum
            {
                ClienteIdPersona = cuenta.ClienteIdPersona,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta == "Ahorro" ? "Ahorro" : "Corriente",
                SaldoInicial = cuenta.SaldoInicial,
                Estado = false,
                FechaApertura = DateTime.Now,
            };
            _db.Cuenta.Add(ef);
            await _db.SaveChangesAsync(ct);
            return ef.CuentaId;
        }

        public Task<bool> NumeroDisponibleAsync(string numeroCuenta, CancellationToken ct)
            => _db.Cuenta.AllAsync(c => c.NumeroCuenta != numeroCuenta || c.Estado, ct);

        public async Task<decimal> ObtenerSaldoActualAsync(long cuentaId, CancellationToken ct)
        {
            decimal baseSaldo, sumMov = 0;
            try
            {
                baseSaldo = await _db.Cuenta
              .Where(c => c.CuentaId == cuentaId && !c.Estado)
              .Select(c => c.SaldoInicial)
              .SingleAsync(ct);

                sumMov = await _db.Movimientos
                   .Where(m => m.CuentaId == cuentaId && !m.Anulado)
                   .SumAsync(m => (decimal?)m.MovimientoNeto, ct) ?? 0m;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Cuenta no existente o en estado inactiva");
            }

            return baseSaldo + sumMov;
        }

        public async Task<decimal> TotalRetirosDelDiaAsync(long cuentaId, DateOnly dia, CancellationToken ct)
        {
            var desde = dia.ToDateTime(TimeOnly.MinValue);
            var hasta = dia.ToDateTime(TimeOnly.MaxValue);

            var total = await _db.Movimientos
                .Where(m => m.CuentaId == cuentaId && !m.Anulado &&
                            m.TipoMovimiento == "D" &&
                            m.Fecha >= desde && m.Fecha <= hasta)
                .SumAsync(m => (decimal?)m.Valor, ct) ?? 0m;

            return total;
        }



        public async Task UpdateAsync(CuentaDomain cuenta, CancellationToken ct)
        {
            var entity = await _db.Cuenta
                .FirstOrDefaultAsync(x => x.CuentaId == cuenta.CuentaId, ct);

            if (entity is null)
            {
                entity = await _db.Cuenta
                    .FirstOrDefaultAsync(x => x.NumeroCuenta == cuenta.NumeroCuenta, ct);
            }

            if (entity is null)
                throw new InvalidOperationException("Cuenta no encontrada para actualizar.");

            entity.TipoCuenta = cuenta.TipoCuenta == "Ahorro" ? "Ahorro" : "Corriente";
            entity.Estado = cuenta.Estado;
            entity.ClienteIdPersona = cuenta.ClienteIdPersona;
            entity.Estado = cuenta.Estado;

            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<CuentaDomain?>> ListAsync(CancellationToken ct)
        {
            var query = from c in _db.Cuenta
                        select new { c };

            var data = await query.ToListAsync(ct);

            return data.Select(x => new CuentaDomain(
                cuentaId: x.c.CuentaId,
                clienteIdPersona: x.c.ClienteIdPersona,
                numeroCuenta: x.c.NumeroCuenta,
                tipoCuenta: x.c.TipoCuenta == "Ahorro" ? Banco.Domain.Enums.CuentaEnum.TipoCuenta.Ahorro : Banco.Domain.Enums.CuentaEnum.TipoCuenta.Corriente,
                saldoInicial: x.c.SaldoInicial,
                estado: x.c.Estado,
                fechaAperturaUtc: x.c.FechaApertura
             

            )).ToList();
        }
    }
}
