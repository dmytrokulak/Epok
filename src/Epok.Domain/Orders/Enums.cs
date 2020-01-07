namespace Epok.Domain.Orders
{
    public enum OrderStatus
    {
        Undefined = 0,
        New = 10,
        Approved = 20,
        InProduction = 30,
        ReadyForShipment = 40,
        Shipped = 50
    }

    public enum OrderType
    {
        Undefined = 0,

        /// <summary>
        /// Usually a suborder 
        /// for an external one 
        /// </summary>
        Internal = 10,

        /// <summary>
        /// For customer
        /// </summary>
        External = 20
    }
}
