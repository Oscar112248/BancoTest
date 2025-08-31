using Banco.Domain.Entities;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Interfaces
{
    public interface IMovimientoRepository
    {
        Task InsertAsync(MovimientoDomain mov, CancellationToken ct);
        Task<IEnumerable<MovimientoDomain>> ObtenerRetirosNoAnuladosAsync(long cuentaId, DateTime desdeUtc, DateTime hastaUtc, CancellationToken ct);

        Task<IEnumerable<MovimientoCuentasDto>> ObtenerMovimientos(CancellationToken ct);

    }
}
