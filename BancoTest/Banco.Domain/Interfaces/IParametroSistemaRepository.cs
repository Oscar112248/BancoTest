using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Interfaces
{
    public interface IParametroSistemaRepository
    {
        Task<decimal> GetLimiteRetiroDiarioAsync(CancellationToken ct);
    }
}
