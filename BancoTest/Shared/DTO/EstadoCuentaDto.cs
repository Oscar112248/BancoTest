using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public sealed record EstadoCuentaDto(
     DateTime Fecha,
     string Cliente,
     string NumeroCuenta,
     string Tipo,
     decimal SaldoInicial,
     bool Estado,           
     decimal Movimiento,
     decimal SaldoDisponible
 );
}
