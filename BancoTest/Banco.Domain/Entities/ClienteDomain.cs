using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Domain.Entities
{
    public class ClienteDomain : PersonaDomain
    {
        public string CodigoCliente { get; }
        public int Edad { get; private set; }
        public bool Estado { get; private set; }

        public string ContraseniaHash { get; private set; }

        public ClienteDomain(long personaId, string nombre, char? genero, int? edad,
                       string identificacion, string? direccion, string? telefono, bool eliminadoPersona,
                       string codigoCliente, string contraseniaHash, bool estado)
            : base(personaId, nombre, genero, edad, identificacion, direccion, telefono, eliminadoPersona)
        {
            CodigoCliente = codigoCliente;
            Edad = (int)edad;
            Estado = estado;
            ContraseniaHash = contraseniaHash;
        }

        public void CambiarEdad(int edadNueva) => Edad = edadNueva;
        public void CambiarDireccion(string direccionNueva) => Direccion = direccionNueva;
        public void CambiarTelefono(string telefonoNuevo) => Telefono = telefonoNuevo;

        public void Activar() => Estado = true;
        public void Desactivar() => Estado = false;
    }
}
