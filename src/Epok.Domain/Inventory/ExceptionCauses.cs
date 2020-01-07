using Epok.Domain.Inventory.Entities;
using Epok.Domain.Orders.Entities;
using Epok.Domain.Shops.Entities;

namespace Epok.Domain.Inventory
{
    public static class ExceptionCauses
    {
        public static string ArchivingArticleStillInStock(Article article, decimal amount)
            => $"{amount} {article.UoM} of {article} is still in stock.";

        public static string ArchivingArticleStillInOrders(Article article, decimal amount)
            => $"{amount} {article.UoM} of {article} is still in active orders.";

        public static string ArticleNotAllowedInShop(Article article, Shop shop)
            => $"{article} is not allowed in {shop}.";

        public static string ArticleNotInOrder(Order order, Article article)
            => $"{article} is not included in {order}.";

        public static string ProductionRequestExceedsRequirements(Order order, decimal amountRequired, Article article,
            decimal amount)
            => $"{order} needs {amountRequired} to fulfill requirements for {article}. {amount} requested.";

        public static string BomIsAlreadyPrimary(BillOfMaterial current)
            => $"{current} of {current.Article} is already set as primary.";

        public static string InsufficientInventory(Article article, decimal required, decimal inStock)
            => $"Required {required} {article.UoM} of {article}; only {inStock} available in stock or can be produced.";

        public static string ArticleNotInStock(Article article, Shop shop)
            => $"{article} is not in {shop} stock.";

        public static string IdenticalBomExists(BillOfMaterial bom)
            => $"Bom with the specified input already exists: {bom}.";

        public static string ArchivingTheOnlyBomForProducibleArticle(BillOfMaterial bom)
            => $"{bom} is the only bom for a producible {bom.Article}";

        public static string ArticleNotProducible(Article article)
            => $"{article} cannot be produced at this factory.";
    }
}
