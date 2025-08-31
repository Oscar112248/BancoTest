using System;
using System.Collections.Generic;

namespace Banco.Infrastructure.Persistence.Entites;

public partial class ParametroSistema
{
    public int ParametroId { get; set; }

    public string Clave { get; set; } = null!;

    public decimal? ValorDecimal { get; set; }
}
