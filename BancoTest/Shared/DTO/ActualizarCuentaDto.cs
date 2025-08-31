using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public sealed class ActualizarCuentaDto
    {
        public string TipoCuenta { get; set; } = default!;
        public bool Estado { get; set; }
        public bool EliminaCuenta { get; set; }

        public long ClienteIdPersona { get; set; }
    }
    public sealed class ActualizarEstadoCuentaDto
    {
        public bool Estado { get; set; }
    }
}
