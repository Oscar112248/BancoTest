using Banco.Domain.Entities;
using Banco.Infrastructure.Persistence.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Infrastructure.Mapper
{
    internal static class MovimientoMapper
    {
        public static MovimientoDomain ToDomain(this Movimiento m)
            => new MovimientoDomain(
                movimientoId: m.MovimientoId,
                cuentaId: m.CuentaId,
                fecha: DateTime.SpecifyKind(m.Fecha, DateTimeKind.Utc),
                tipo: m.TipoMovimiento == "C" ? TipoMovimiento.Credito : TipoMovimiento.Debito,
                valorAbsoluto: m.Valor,
                movimientoNeto: m.MovimientoNeto,
                anulado: m.Anulado
            );
    }
}
