using Banco.Domain.Entities;
using Banco.Infrastructure.Persistence.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Infrastructure.Mapper
{
    internal static class CuentaMapper
    {
        public static CuentaDomain ToDomain(this Cuentum c)
            => new CuentaDomain(
                cuentaId: c.CuentaId,
                clienteIdPersona: c.ClienteIdPersona,
                numeroCuenta: c.NumeroCuenta,
                tipoCuenta: c.TipoCuenta == "Ahorro" ? TipoCuenta.Ahorro : TipoCuenta.Corriente,
                saldoInicial: c.SaldoInicial,
                estado: c.Estado,
                fechaAperturaUtc: DateTime.SpecifyKind(c.FechaApertura, DateTimeKind.Utc)
            );
    }
}
