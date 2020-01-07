using Epok.Core.Domain.Commands;

namespace Epok.Domain.Customers.Commands
{
    /// <summary>
    /// Registers new customer with the system.
    /// </summary>
    public class RegisterCustomer : CreateEntityCommand
    {
        public CustomerType Type { get; set; }
        public string PrimaryContactFirstName { get; set; }
        public string PrimaryContactLastName { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressProvince { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string ShippingAddressPostalCode { get; set; }
    }
}
