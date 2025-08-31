using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Exceptions
{
    public sealed class SaldoInsuficienteException : DomainException
    {
        public SaldoInsuficienteException()
            : base("MOV_002", "Saldo no disponible.")
        {
        }
    }
}
