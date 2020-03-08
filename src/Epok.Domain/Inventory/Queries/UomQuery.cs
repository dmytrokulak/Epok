using Epok.Core.Domain.Queries;

namespace Epok.Domain.Inventory.Queries
{
    public class UomQuery : QueryBase
    {
        public UomType? FilterTypeExact { get; set; }
    }
}
