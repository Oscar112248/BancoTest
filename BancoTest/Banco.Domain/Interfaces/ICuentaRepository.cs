using Banco.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Interfaces
{
    public interface ICuentaRepository
    {
        Task<CuentaDomain?> GetByNumeroAsync(string numeroCuenta, bool solicita, CancellationToken ct);
        Task<CuentaDomain?> GetByIdAsync(long cuentaId, CancellationToken ct);
        Task<long> CrearAsync(CuentaDomain cuenta, CancellationToken ct);
        Task<bool> NumeroDisponibleAsync(string numeroCuenta, CancellationToken ct);

        Task<decimal> ObtenerSaldoActualAsync(long cuentaId, CancellationToken ct);
        Task<decimal> TotalRetirosDelDiaAsync(long cuentaId, DateOnly dia, CancellationToken ct);
        Task UpdateAsync(CuentaDomain cuenta, CancellationToken ct);
        Task<List<CuentaDomain?>> ListAsync(CancellationToken ct);

    }
}
