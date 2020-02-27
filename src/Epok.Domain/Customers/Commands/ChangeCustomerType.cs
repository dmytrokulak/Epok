using System;
using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Changes customer's type.
    /// Throws an exception if NewCustomerType is undefined.
    /// </summary>
    public class ChangeCustomerType : CommandBase
    {
        public Guid Id { get; set; }
        public CustomerType NewCustomerType { get; set; }
    }
}
