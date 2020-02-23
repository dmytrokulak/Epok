using System;
using System.Collections.Generic;

namespace Epok.Core.Domain.Queries
{
    public class QueryBase : IQuery
    {
        public ICollection<Guid> FilterIds { get; set; }
        public string FilterName { get; set; }
        public bool Lazy { get; private set; }

        public QueryBase AsLazy() 
        {
            Lazy = true;
            return this;
        }
    }
}
