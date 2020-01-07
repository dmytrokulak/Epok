
namespace Epok.Domain.Inventory
{
    /// <summary>
    /// According to its readiness 
    /// to be shipped to end customer.
    /// </summary>
    public enum ArticleType
    {
        Undefined = 0,
        Material = 10,
        SuppliedComponent = 20,
        ManufacturedComponent = 30,
        Product = 40,
        ProductToShip = 50
    }

    public enum UomType
    {
        Undefined = 0,
        Piece = 1,

        //base points in meters 
        Length = 2,

        //base points in sq meters
        Area = 3,

        //base points in cub meters
        Volume = 4,

        //base points in grams
        Weight = 5,
    }
}
