using System;
using System.Collections.Generic;

namespace Epok.Core.Domain.Queries
{
    public class QueryBase : IQuery
    {
        public int? Take { get; set; }
        public int? Skip { get; set; }
        public string OrderBy { get; set; }
        public string OrderMode { get; set; }
        public ICollection<Guid> FilterIds { get; set; }
        public string FilterNameLike { get; set; }
        public bool Lazy { get; set; }

        public void AsLazy() => Lazy = true;
    }
}
