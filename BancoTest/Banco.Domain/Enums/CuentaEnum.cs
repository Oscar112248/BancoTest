using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Enums
{
    public sealed class CuentaEnum
    {
        public enum TipoCuenta { Ahorro, Corriente }
        public enum TipoMovimiento { Credito, Debito }
    }
}
