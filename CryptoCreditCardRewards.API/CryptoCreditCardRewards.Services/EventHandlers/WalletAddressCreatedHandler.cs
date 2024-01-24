using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Events;
using CryptoCreditCardRewards.Services.Entity;
using CryptoCreditCardRewards.Services.Entity.Interfaces;

namespace CryptoCreditCardRewards.Services.EventHandlers
{
    public class WalletAddressCreatedHandler : INotificationHandler<CreatedWalletAddressEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public WalletAddressCreatedHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Handle(CreatedWalletAddressEvent notification, CancellationToken cancellationToken)
        {
            var userRewardSelectionService = _serviceProvider.GetRequiredService<IUserRewardSelectionService>();

            // Create a reward selection for the new wallet address 
            return userRewardSelectionService.CreateUserRewardSelectionAsync(notification.UserId, notification.CryptoCurrencyId);
        }
    }
}
