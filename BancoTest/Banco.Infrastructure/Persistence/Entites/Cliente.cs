using System;
using System.Collections.Generic;

namespace Banco.Infrastructure.Persistence.Entites;

public partial class Cliente
{
    public long ClienteIdPersona { get; set; }

    public string CodigoCliente { get; set; } = null!;

    public string Contrasenia { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual Persona ClienteIdPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();
}
