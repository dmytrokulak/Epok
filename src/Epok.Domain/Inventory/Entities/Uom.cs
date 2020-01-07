using Epok.Core.Domain.Entities;
using System;

namespace Epok.Domain.Inventory.Entities
{
    /// <summary>
    /// Unit of measurement with a reference
    /// to a base points benchmark for converting
    /// between different units.
    /// </summary>
    [Serializable]
    public class Uom : EntityBase
    {
        public Uom(Guid id, string name, UomType uomType) : base(id, name)
        {
            Type = uomType;
        }

        /// <summary>
        /// Piece, length, weight etc.
        /// </summary>
        public UomType Type { get; set; }

        /// <summary>
        /// Conversion to benchmark measurement
        /// </summary>
        public decimal BasePoints { get; set; }

        /// <summary>
        /// Whether this UoM is one from
        /// International System of Units of Measurements
        /// or a custom (non-metric) one.
        /// </summary>
        public bool Custom { get; set; }
    }
}