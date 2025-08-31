using Banco.Domain.Entities;
using Banco.Domain.Interfaces;
using Banco.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Banco.Infrastructure.Mapper;
using Banco.Infrastructure.Persistence.Entites;


namespace Banco.Infrastructure.Persistence.Repositories
{
    public sealed class ClienteRepository : IClienteRepository
    {
        private readonly BANCO_DbContext _db;
        public ClienteRepository(BANCO_DbContext db) => _db = db;

        public async Task<ClienteDomain?> GetByPersonaIdAsync(long personaId, CancellationToken ct)
        {
            var efCliente = await _db.Clientes
                .Where(c => c.ClienteIdPersona == personaId && !c.Estado)
                .SingleOrDefaultAsync(ct);

            if (efCliente is null) return null;

            var efPersona = await _db.Personas
                .SingleAsync(p => p.PersonaId == personaId && !p.EliminadoPersona, ct);

            return ClienteMapper.ToDomain(efCliente, efPersona);

        }

        public async Task<ClienteDomain?> GetByCodigoAsync(string codigo, CancellationToken ct)
        {
            var efCliente = await _db.Clientes
                .SingleOrDefaultAsync(c => c.CodigoCliente == codigo && !c.Estado, ct);
            if (efCliente is null) return null;

            var efPersona = await _db.Personas
                .SingleAsync(p => p.PersonaId == efCliente.ClienteIdPersona && !p.EliminadoPersona, ct);

            return ClienteMapper.ToDomain(efCliente, efPersona);


        }

        public Task<bool> ExisteIdentificacionAsync(string identificacion, CancellationToken ct)
            => _db.Personas.AnyAsync(p => p.Identificacion == identificacion && !p.EliminadoPersona, ct);

        public Task<bool> ExisteCodigoClienteAsync(string codigo, CancellationToken ct)
            => _db.Clientes.AnyAsync(c => c.CodigoCliente == codigo && !c.Estado, ct);

        public async Task<long> CrearPersonaYClienteAsync(PersonaDomain persona, ClienteDomain cliente, CancellationToken ct)
        {
            using var trx = await _db.Database.BeginTransactionAsync(ct);

            var efPersona = new Entites.Persona
            {
                Nombre = persona.Nombre,
                Genero = persona.Genero.HasValue ? persona.Genero.Value.ToString() : null,
                Edad = persona.Edad,
                Identificacion = persona.Identificacion,
                Direccion = persona.Direccion,
                Telefono = persona.Telefono,
                EliminadoPersona = false
            };
            _db.Personas.Add(efPersona);
            await _db.SaveChangesAsync(ct);

            var efCliente = new Entites.Cliente
            {
                ClienteIdPersona = efPersona.PersonaId,
                CodigoCliente = cliente.CodigoCliente,
                Contrasenia = cliente.ContraseniaHash,
                Estado = false
            };
            _db.Clientes.Add(efCliente);
            await _db.SaveChangesAsync(ct);

            await trx.CommitAsync(ct);
            return efPersona.PersonaId;
        }

        public async Task UpdateAsync(ClienteDomain cliente, CancellationToken ct)
        {
            var entity = await _db.Clientes
                .FirstOrDefaultAsync(x => x.CodigoCliente == cliente.CodigoCliente, ct);

            entity.ClienteIdPersonaNavigation.Edad = cliente.Edad;
            entity.Estado = cliente.Estado;
            entity.ClienteIdPersonaNavigation.Telefono = cliente.Telefono;
            entity.ClienteIdPersonaNavigation.Direccion = cliente.Direccion;

            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<ClienteDomain>> ListAsync(CancellationToken ct)
        {
            
            var query = from c in _db.Clientes
                        join p in _db.Personas on c.ClienteIdPersona equals p.PersonaId
                         
                        select new { c, p };

            var data = await query.ToListAsync(ct);

            return data.Select(x => new ClienteDomain(
                personaId: x.p.PersonaId,
                nombre: x.p.Nombre,
                genero: string.IsNullOrWhiteSpace(x.p.Genero) ? null : x.p.Genero![0],
                edad: x.p.Edad,
                identificacion: x.p.Identificacion,
                direccion: x.p.Direccion,
                telefono: x.p.Telefono,
                eliminadoPersona: x.p.EliminadoPersona,
                codigoCliente: x.c.CodigoCliente,
                contraseniaHash: x.c.Contrasenia,
                estado: x.c.Estado
            )).ToList();
        }

        public async  Task SoftDeleteAsync(string personaId, CancellationToken ct)
        {
            var entity = await _db.Clientes
                .FirstOrDefaultAsync(x => x.CodigoCliente == personaId, ct);

            entity.Estado = true;

            await _db.SaveChangesAsync(ct);
        }
    }
}
