using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public sealed class ActualizaClienteDto
    {
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        public int Edad { get; set; }
    }
}
