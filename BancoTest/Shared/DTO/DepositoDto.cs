using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{

    public sealed class DepositoDto
    {
        [Required, StringLength(20)] public string NumeroCuenta { get; set; } = null!;
        [Range(0.01, double.MaxValue)] public decimal Valor { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
