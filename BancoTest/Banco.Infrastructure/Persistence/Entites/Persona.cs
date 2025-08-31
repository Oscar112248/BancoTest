using System;
using System.Collections.Generic;

namespace Banco.Infrastructure.Persistence.Entites;

public partial class Persona
{
    public long PersonaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Genero { get; set; }

    public int? Edad { get; set; }

    public string Identificacion { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public bool EliminadoPersona { get; set; }

    public virtual Cliente? Cliente { get; set; }
}
