using Epok.Domain.Users.Entities;
using System;

namespace Epok.Domain.Users
{
    public static class ExceptionCauses
    {
        public static readonly Func<User, string> ArchivingShopManager
            = user => $"Cannot archive a manager {user.Id} of shop {user.Shop.Id}.";

        public static readonly Func<Permission, string> DuplicatingGrant
            = permission => $"Permission {permission.Id} on handler {permission.Handler.Id} " +
                            $"is already granted to user {permission.User.Id}.";

        public static readonly Func<User, string> RevokingGlobalAdminPermission
            = user => $"Cannot revoke global admin's permissions. Admin user id {user.Id}.";
    }
}
