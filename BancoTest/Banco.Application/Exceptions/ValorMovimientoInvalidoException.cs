using Banco.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Application.Exceptions
{
    public sealed class ValorMovimientoInvalidoException : DomainException
    {
        public ValorMovimientoInvalidoException()
            : base("MOV_001", "El valor del movimiento debe ser mayor a cero.")
        {
        }
    }
}
