
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banco.Domain.Entities;
using Banco.Infrastructure.Persistence.Entites;

namespace Banco.Infrastructure.Mapper
{
    public class ClienteMapper
    {
        public static ClienteDomain ToDomain(Cliente efCliente, Persona efPersona)
              => new ClienteDomain(
                  personaId: efPersona.PersonaId,
                  nombre: efPersona.Nombre,
                    genero: string.IsNullOrWhiteSpace(efPersona.Genero) ? null : efPersona.Genero![0],
                  edad: efPersona.Edad,
                  identificacion: efPersona.Identificacion,
                  direccion: efPersona.Direccion,
                  telefono: efPersona.Telefono,
                  eliminadoPersona: efPersona.EliminadoPersona,
                  codigoCliente: efCliente.CodigoCliente,
                  contraseniaHash: efCliente.Contrasenia,
                  estado: efCliente.Estado
              );

        public static List<ClienteDomain> ToDomainList(Cliente efCliente, Persona efPersona)
        {
            return new List<ClienteDomain>
    {
        new ClienteDomain(
            personaId: efPersona.PersonaId,
            nombre: efPersona.Nombre,
            genero: string.IsNullOrWhiteSpace(efPersona.Genero) ? null : efPersona.Genero![0],
            edad: efPersona.Edad,
            identificacion: efPersona.Identificacion,
            direccion: efPersona.Direccion,
            telefono: efPersona.Telefono,
            eliminadoPersona: efPersona.EliminadoPersona,
            codigoCliente: efCliente.CodigoCliente,
            contraseniaHash: efCliente.Contrasenia,
            estado: efCliente.Estado
        )
    };
        }

    }
}
