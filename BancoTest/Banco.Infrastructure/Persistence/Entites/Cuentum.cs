using System;
using System.Collections.Generic;

namespace Banco.Infrastructure.Persistence.Entites;

public partial class Cuentum
{
    public long CuentaId { get; set; }

    public long ClienteIdPersona { get; set; }

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public decimal SaldoInicial { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaApertura { get; set; }

    public virtual Cliente ClienteIdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
