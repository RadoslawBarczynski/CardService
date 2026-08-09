using CardService.Api.Models;
using Domain;
using Domain.Enums;
using Domain.Services;

namespace CardService.Tests
{
    public class AllowedActionsResolverTests
    {
        private readonly AllowedActionsResolver _resolver = new();

        [Fact]
        public void Prepaid_Closed_Returns_Action3_4_9()
        {
            var card = new CardDetails("xyz", CardType.Prepaid, CardStatus.Closed, IsPinSet: false);

            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(
                new[] 
                { 
                    ActionType.ACTION3, ActionType.ACTION4, ActionType.ACTION9 
                }, 
                    actions
                );
        }

        [Fact]
        public void Credit_Blocked_WithPin_Returns_ExpectedActions()
        {
            var card = new CardDetails("xyz", CardType.Credit, CardStatus.Blocked, IsPinSet: true);

            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(
                new[]
                {
                    ActionType.ACTION3, ActionType.ACTION4, ActionType.ACTION5,
                    ActionType.ACTION6, ActionType.ACTION7, ActionType.ACTION8,
                    ActionType.ACTION9
                },
                    actions
                );
        }
    }
}