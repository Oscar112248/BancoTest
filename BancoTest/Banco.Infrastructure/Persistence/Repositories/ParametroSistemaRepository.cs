using Banco.Domain.Interfaces;
using Banco.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Infrastructure.Persistence.Repositories
{
    public sealed class ParametroSistemaRepository : IParametroSistemaRepository
    {
        private readonly BANCO_DbContext _db;

        public ParametroSistemaRepository(BANCO_DbContext db)
        {
            _db = db;
        }
        public async Task<decimal> GetLimiteRetiroDiarioAsync(CancellationToken ct)
        {
            return (decimal)await _db.ParametroSistemas.Where(x => x.Clave == "LIMITE_RETIRO_DIARIO").Select(x => x.ValorDecimal).FirstOrDefaultAsync(ct);
        }
    }
}
