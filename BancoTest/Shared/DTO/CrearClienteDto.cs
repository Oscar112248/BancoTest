using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public sealed class CrearClienteDto
    {
        [Required, StringLength(120)]
        public string Nombre { get; set; } = null!;

        [RegularExpression("^[MFO]$")]
        public char? Genero { get; set; }

        [Range(0, 130)]
        public int? Edad { get; set; }

        [Required, StringLength(20)]
        public string Identificacion { get; set; } = null!;

        [StringLength(250)]
        public string? Direccion { get; set; }

        [StringLength(25)]
        public string? Telefono { get; set; }

        [Required, StringLength(20)]
        public string CodigoCliente { get; set; } = null!;

        
        [Required, StringLength(256)]
        public string ContraseniaHash { get; set; } = null!;
    }
}
