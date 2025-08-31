using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Banco.Domain.Enums.CuentaEnum;

namespace Banco.Domain.Entities
{
    public class CuentaDomain
    {
        public long CuentaId { get; }
        public long ClienteIdPersona { get; private set; }
        public string NumeroCuenta { get; }
        public string TipoCuenta { get; private set; }
        public decimal SaldoInicial { get; }
        public bool Estado { get; private set; }
        public DateTime FechaAperturaUtc { get; }

        public CuentaDomain(
            long cuentaId, long clienteIdPersona, string numeroCuenta, TipoCuenta tipoCuenta,
            decimal saldoInicial, bool estado, DateTime fechaAperturaUtc)
        {
            CuentaId = cuentaId;
            ClienteIdPersona = clienteIdPersona;
            NumeroCuenta = numeroCuenta;
            TipoCuenta = tipoCuenta == 0 ? "Ahorro" : "Corriente";
            SaldoInicial = saldoInicial;
            Estado = estado;
            FechaAperturaUtc = fechaAperturaUtc;
        }

        public void CambiarTipoCuenta(TipoCuenta nuevo) => TipoCuenta = nuevo == 0 ? "Ahorro" : "Corriente";


        public void Activar() => Estado = true;
        public void Desactivar() => Estado = false;

    }
}
