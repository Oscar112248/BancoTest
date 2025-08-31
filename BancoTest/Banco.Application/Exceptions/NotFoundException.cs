using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Application.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} con identificador '{key}' no fue encontrado.")
        {
        }
    }
}
