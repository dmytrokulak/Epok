using Epok.Core.Domain.Commands;
using System;

namespace Epok.Domain.Suppliers.Commands
{
    /// <summary>
    /// Receives materials from the supplier.
    /// </summary>
    public class ReceiveMaterials : CommandBase
    {
        public Guid MaterialRequestId { get; set; }
    }
}
