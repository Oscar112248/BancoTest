using Banco.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Application.Exceptions
{
    public sealed class CuentaNoEncontradaException : DomainException
    {
        public CuentaNoEncontradaException(string numero)
            : base("CTA_404", $"Cuenta '{numero}' no encontrada.") { }
    }

    public sealed class CuentaInactivaException : DomainException
    {
        public CuentaInactivaException(string numero)
            : base("CTA_002", $"La cuenta '{numero}' está inactiva.") { }
    }

    public sealed class ClienteExisteException : DomainException
    {
        public ClienteExisteException(string identificacion)
            : base("CLI_001", $"El cliente con identificación '{identificacion}' ya existe.") { }
    }

    public sealed class CodigoExisteException : DomainException
    {
        public CodigoExisteException(string codigo)
            : base("COD_001", $"El código del cliente  {codigo}' ya existe.") { }
    }
    public sealed class CuentaExisteException : DomainException
    {
        public CuentaExisteException(string cuenta)
            : base("CTA_006", $"La cuenta  {cuenta}' ya existe.") { }
    }


}
