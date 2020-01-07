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

        public bool IsShopManager { get; set; }

        /// <summary>
        /// The shop user may be assigned to. 
        /// </summary>
        public Shop Shop { get; set; }

        public UserType UserType { get; set; }
        public string Email { get; set; }
    }
}
