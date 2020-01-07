using Epok.Core.Domain.Persistence;
using Epok.Domain.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Epok.Domain.Inventory.Repositories
{
    /// <summary>
    /// Local default for unit of measures repository.
    /// </summary>
    /// <remarks>
    /// Not clear whether units of measures are going to
    /// be stored in a database.
    /// </remarks>
    public class UomRepository : IRepository<Uom>
    {
        private readonly Dictionary<Guid, Uom> _commonUoms = new Dictionary<Guid, Uom>()
        {
            {
                Guid.Parse("00000000-0000-0000-0008-010510199101"),
                new Uom(Guid.Parse("00000000-0000-0000-0008-010510199101"), "Piece", UomType.Piece)
            },
            {
                Guid.Parse("00000000-0000-0000-0077-101116101114"),
                new Uom(Guid.Parse("00000000-0000-0000-0077-101116101114"), "Meter", UomType.Length)
            },
            {
                Guid.Parse("13117097-1141-0103-2077-101116101114"),
                new Uom(Guid.Parse("13117097-1141-0103-2077-101116101114"), "Square Meter", UomType.Area)
            },
            {
                Guid.Parse("71177098-1050-9903-2077-101116101114"),
                new Uom(Guid.Parse("71177098-1050-9903-2077-101116101114"), "Cubic Meter", UomType.Volume)
            },
            {
                Guid.Parse("00000000-0000-0000-0000-007111497109"),
                new Uom(Guid.Parse("00000000-0000-0000-0000-007111497109"), "Gram", UomType.Weight)
            },
            {
                Guid.Parse("00077105-1081-0810-5109-101116101114"),
                new Uom(Guid.Parse("00077105-1081-0810-5109-101116101114"), "Millimeter", UomType.Length)
                    {BasePoints = 0.001M}
            },
            {
                Guid.Parse("00067101-1101-1610-5109-101116114101"),
                new Uom(Guid.Parse("00067101-1101-1610-5109-101116114101"), "Centimeter", UomType.Length)
                    {BasePoints = 0.01M}
            },
            {
                Guid.Parse("00000000-0751-0510-8111-103114097109"),
                new Uom(Guid.Parse("00000000-0751-0510-8111-103114097109"), "Kilogram", UomType.Weight)
                    {BasePoints = 1000}
            },
        };

        public Task AddAsync(Uom entity)
        {
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<Uom>> GetAllAsync()
        {
            return await Task.FromResult(_commonUoms.Values);
        }

        public async Task<Uom> GetAsync(Guid id)
        {
            return await Task.FromResult(_commonUoms[id]);
        }

        public async Task<Uom> LoadAsync(Guid id)
        {
            return await Task.FromResult(_commonUoms[id]);
        }

        public Task ArchiveAsync(Guid id)
        {
            throw new NotSupportedException();
        }

        public Task AddRangeAsync(Uom entity)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveRangeAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Uom>> GetSomeAsync(IEnumerable<Guid> id)
        {
            throw new NotImplementedException();
        }
    }
}
