using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Exceptions
{
    public sealed class LimiteDiarioExcedidoException : DomainException
    {
        public LimiteDiarioExcedidoException()
            : base("MOV_003", "Cupo diario de retiros excedido.")
        {
        }
    }
}
