using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Entities
{
    public class PersonaDomain
    {
        public long PersonaId { get; }
        public string Nombre { get; }
        public char? Genero { get; }
        public int? Edad { get; }
        public string Identificacion { get; }
        public string? Direccion { get;  set; }
        public string? Telefono { get;   set; }
        public bool Eliminado { get; private set; }

        public PersonaDomain(long personaId, string nombre, char? genero, int? edad,
                       string identificacion, string? direccion, string? telefono, bool eliminado)
        {
            PersonaId = personaId;
            Nombre = nombre;
            Genero = genero;
            Edad = edad;
            Identificacion = identificacion;
            Direccion = direccion;
            Telefono = telefono;
            Eliminado = eliminado;
        }

        public void MarcarEliminado() => Eliminado = true;
    }
}
