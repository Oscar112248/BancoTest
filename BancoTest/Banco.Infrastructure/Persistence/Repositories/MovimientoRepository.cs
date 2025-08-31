using Banco.Domain.Entities;
using Banco.Domain.Interfaces;
using Banco.Infrastructure.Persistence.Context;
using Banco.Infrastructure.Persistence.Entites;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Infrastructure.Persistence.Repositories
{
    public sealed class MovimientoRepository : IMovimientoRepository
    {
        private readonly BANCO_DbContext _db;

        public MovimientoRepository(BANCO_DbContext db)
        {
            _db = db;
        }
     

        public async Task InsertAsync(MovimientoDomain mov, CancellationToken ct)
        {
            var entity = new Movimiento
            {
                CuentaId = mov.CuentaId,
                Fecha = mov.Fecha,
                TipoMovimiento = mov.Tipo == Banco.Domain.Enums.CuentaEnum.TipoMovimiento.Credito ? "C" : "D",
                Valor = mov.ValorAbsoluto,
                Anulado = mov.Anulado
            };

            _db.Movimientos.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<MovimientoCuentasDto>> ObtenerMovimientos(CancellationToken ct)
        {
            var query = from c in _db.Cuenta
                        join p in _db.Personas on c.ClienteIdPersona equals p.PersonaId
                        join m in _db.Movimientos on c.CuentaId equals m.CuentaId into movs
                        from m in movs.DefaultIfEmpty() 
                        select new MovimientoCuentasDto(
                            c.NumeroCuenta,
                            c.TipoCuenta,
                            c.SaldoInicial,
                            m != null && m.Anulado,               
                            (decimal)(m != null ? m.MovimientoNeto : 0m),    
                            m != null ? m.TipoMovimiento : string.Empty 
                        );
            return await query.ToListAsync(ct);

        }

        public async Task<IEnumerable<MovimientoDomain>> ObtenerRetirosNoAnuladosAsync(long cuentaId, DateTime desdeUtc, DateTime hastaUtc, CancellationToken ct)
        {
            return await _db.Movimientos
                .Where(m => m.CuentaId == cuentaId
                            && m.Anulado == false
                            && m.TipoMovimiento == "D"
                            && m.Fecha >= desdeUtc
                            && m.Fecha < hastaUtc)
                .Select(m => new MovimientoDomain(
                    m.MovimientoId,
                    m.CuentaId,
                    m.Fecha,
                    m.TipoMovimiento == "C"
                        ? Banco.Domain.Enums.CuentaEnum.TipoMovimiento.Credito
                        : Banco.Domain.Enums.CuentaEnum.TipoMovimiento.Debito,
                    m.Valor,
                    m.MovimientoNeto,
                    m.Anulado
                ))
                .ToListAsync(ct);
        }
    }
}
