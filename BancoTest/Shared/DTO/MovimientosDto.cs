using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public sealed record RealizarDepositoCommand(string NumeroCuenta, decimal Valor, DateTime? Fecha);
    public sealed record RealizarRetiroCommand(string NumeroCuenta, decimal Valor, DateTime? Fecha);
   

    public sealed record MovimientoCreadoResult(long MovimientoId, string NumeroCuenta, decimal Valor, DateTime Fecha, decimal SaldoDisponible);
    
    public sealed record MovimientoCuentasDto( string numeroCuenta, string tipoCuenta, decimal saldoInicial, bool anulado, decimal movimientoNeto, string tipoMovimiento);
    public sealed record EstadoCuentaResult(object Cliente, IEnumerable<object> Cuentas, object Totales, string? PdfBase64);
}
