using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Entities
{
    public sealed class ParametroSistema
    {
        public int ParametroId { get; set; }
        public string Clave { get; set; } = default!;
        public decimal? ValorDecimal { get; set; }
       
    }
}
