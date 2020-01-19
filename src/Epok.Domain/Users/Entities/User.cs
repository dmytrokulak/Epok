using Epok.Core.Domain.Entities;
using Epok.Domain.Shops.Entities;
using System;

namespace Epok.Domain.Users.Entities
{
    /// <summary>
    /// User of the system.
    /// </summary>
    [Serializable]
    public class User : EntityBase
    {
        public User(Guid id, string name) : base(id, name)
        {
        }

        public virtual bool IsShopManager { get; set; }

        /// <summary>
        /// The shop user may be assigned to. 
        /// </summary>
        public virtual Shop Shop { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual string Email { get; set; }
    }
}
