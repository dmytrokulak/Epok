using System;
using System.Collections.Generic;

namespace Epok.Core.Domain.Queries
{
    public class QueryBase : IQuery
    {
        public ICollection<Guid> FilterIds { get; set; }
        public string FilterNameLike { get; set; }
        public bool Lazy { get; set; }

        public void AsLazy() => Lazy = true;
    }
}
