using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Domain.Entities
{
    public class MovimientoDomain
    {
        public long MovimientoId { get; }
        public long CuentaId { get; }
        public DateTime Fecha { get; }
        public TipoMovimiento Tipo { get; }
        public decimal ValorAbsoluto { get; }     
        public decimal? MovimientoNeto { get; }     
        public bool Anulado { get; private set; }

        public MovimientoDomain(long movimientoId, long cuentaId, DateTime fecha,
                          TipoMovimiento tipo, decimal valorAbsoluto, decimal? movimientoNeto, bool anulado)
        {
            MovimientoId = movimientoId;
            CuentaId = cuentaId;
            Fecha = fecha;
            Tipo = tipo;
            ValorAbsoluto = valorAbsoluto;
            MovimientoNeto = movimientoNeto;
            Anulado = anulado;
        }

        public void Anular() => Anulado = true;

        public static MovimientoDomain CrearCredito(long cuentaId, decimal valor, DateTime? fecha = null)
        {
            if (valor <= 0) throw new ArgumentException("El valor debe ser mayor que cero.", nameof(valor));

            var f = fecha.HasValue
                ? (fecha.Value.Kind == DateTimeKind.Utc ? fecha.Value
                                                           : DateTime.SpecifyKind(fecha.Value, DateTimeKind.Utc))
                : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            return new MovimientoDomain(
                movimientoId: 0,
                cuentaId: cuentaId,
                fecha: f,
                tipo: TipoMovimiento.Credito,
                valorAbsoluto: valor,
                movimientoNeto: valor,
                anulado: false
            );
        }

       
        public static MovimientoDomain CrearDebito(long cuentaId, decimal valor, DateTime? fecha = null)
        {
            if (valor <= 0) throw new ArgumentException("El valor debe ser mayor que cero.", nameof(valor));

            var f = fecha.HasValue
                ? (fecha.Value.Kind == DateTimeKind.Utc ? fecha.Value
                                                           : DateTime.SpecifyKind(fecha.Value, DateTimeKind.Utc))
                : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            return new MovimientoDomain(
                movimientoId: 0,
                cuentaId: cuentaId,
                fecha: f,
                tipo: TipoMovimiento.Debito,
                valorAbsoluto: valor,
                movimientoNeto: -valor,
                anulado: false
            );
        }
    }
}
