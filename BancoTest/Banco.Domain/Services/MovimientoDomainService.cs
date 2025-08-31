using Banco.Domain.Entities;
using Banco.Domain.Exceptions;
using Banco.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Services
{
    public sealed class MovimientoDomainService
    {
        private readonly IParametroSistemaRepository _param;

        public MovimientoDomainService(IParametroSistemaRepository param)
        { _param = param; }

        public MovimientoDomain Depositar(CuentaDomain cuenta, decimal valor, DateTime? fechaUtc = null)
        {
            var f = fechaUtc ?? DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            return MovimientoDomain.CrearCredito(cuenta.CuentaId, valor, f);
        }

        public async Task<MovimientoDomain> Retirar(
            CuentaDomain cuenta, decimal valor, decimal saldoActual,
            IEnumerable<MovimientoDomain> retirosDelDiaNoAnulados, DateTime? fechaUtc = null)
        {
            if (saldoActual - valor < 0) throw new SaldoInsuficienteException();

            var limite = await _param.GetLimiteRetiroDiarioAsync(CancellationToken.None);
            var acumulado = retirosDelDiaNoAnulados.Sum(m => m.ValorAbsoluto);
            if (acumulado + valor > limite)
                throw new LimiteDiarioExcedidoException();

            var f = fechaUtc ?? DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            return MovimientoDomain.CrearDebito(cuenta.CuentaId, valor, f);
        }

    }
}
