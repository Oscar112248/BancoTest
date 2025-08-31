using Banco.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<ClienteDomain?> GetByPersonaIdAsync(long personaId, CancellationToken ct);
        Task<ClienteDomain?> GetByCodigoAsync(string codigo, CancellationToken ct);
        Task<bool> ExisteIdentificacionAsync(string identificacion, CancellationToken ct);
        Task<bool> ExisteCodigoClienteAsync(string codigo, CancellationToken ct);
        Task<long> CrearPersonaYClienteAsync(PersonaDomain persona, ClienteDomain cliente, CancellationToken ct);
        Task UpdateAsync(ClienteDomain cliente, CancellationToken ct);
        Task SoftDeleteAsync(string personaId, CancellationToken ct);


        Task<List<ClienteDomain?>> ListAsync(CancellationToken ct);

    }
}
