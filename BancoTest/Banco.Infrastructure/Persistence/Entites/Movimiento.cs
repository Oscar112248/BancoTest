using System;
using System.Collections.Generic;

namespace Banco.Infrastructure.Persistence.Entites;

public partial class Movimiento
{
    public long MovimientoId { get; set; }

    public long CuentaId { get; set; }

    public DateTime Fecha { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public decimal Valor { get; set; }

    public decimal? MovimientoNeto { get; set; }

    public bool Anulado { get; set; }

    public virtual Cuentum Cuenta { get; set; } = null!;
}
