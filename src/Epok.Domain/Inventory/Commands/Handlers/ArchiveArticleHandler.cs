﻿using Epok.Core.Domain.Commands;
using Epok.Core.Domain.Events;
using Epok.Core.Domain.Exceptions;
using Epok.Domain.Inventory.Entities;
using Epok.Domain.Inventory.Repositories;
using System.Threading.Tasks;
using static Epok.Domain.Inventory.ExceptionCauses;

namespace Epok.Domain.Inventory.Commands.Handlers
{
    /// <summary>
    /// Archives article if no corresponding inventory is in stock
    /// and article is not in active orders.
    /// </summary>
    /// <exception cref="DomainException">
    /// If inventory or active orders for this article exist.
    /// </exception>
    public class ArchiveArticleHandler : ICommandHandler<ArchiveArticle>
    {
        private readonly IArticleRepository _articleRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IEventTransmitter _eventTransmitter;

        public ArchiveArticleHandler(IInventoryRepository inventoryRepo,
            IArticleRepository articleRepo, IEventTransmitter eventTransmitter)
        {
            _inventoryRepo = inventoryRepo;
            _articleRepo = articleRepo;
            _eventTransmitter = eventTransmitter;
        }

        public async Task HandleAsync(ArchiveArticle command)
        {
            var article = await _articleRepo.LoadAsync(command.Id);

            var amountInStock = await _inventoryRepo.FindTotalAmountInStockAsync(article);
            if (amountInStock > 0)
                throw new DomainException(ArchivingArticleStillInStock(article, amountInStock));

            var amountInOrders = await _inventoryRepo.FindTotalAmountInOrdersAsync(article);
            if (amountInOrders > 0)
                throw new DomainException(ArchivingArticleStillInOrders(article, amountInOrders));

            await _inventoryRepo.ArchiveAsync(command.Id);
            await _eventTransmitter.BroadcastAsync(new DomainEvent<Article>(article, Trigger.Removed,
                command.InitiatorId));
        }
    }
}