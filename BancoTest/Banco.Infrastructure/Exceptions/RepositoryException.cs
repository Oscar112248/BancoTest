using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Infrastructure.Exceptions
{
    public sealed class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
