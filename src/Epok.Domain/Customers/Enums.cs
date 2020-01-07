
namespace Epok.Domain.Customers
{
    /// <summary>
    /// According to type\scale of business.
    /// </summary>
    public enum CustomerType
    {
        Undefined = 0,
        Individual = 10,
        OnlineStore = 20,
        RetailOutlet = 30,
        RetailNetwork = 40,
        WholesaleDealer = 50
    }
}
