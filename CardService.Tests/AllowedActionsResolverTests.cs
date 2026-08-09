using CardService.Api.Models;
using CardService.Domain.Enums;
using CardService.Domain.Services;

namespace CardService.Tests
{
    public class AllowedActionsResolverTests
    {
        private readonly AllowedActionsResolver _resolver = new();

        #region Facts

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

        #endregion Facts

        #region Theories

        [Theory]
        [InlineData(CardType.Prepaid, false)]
        [InlineData(CardType.Debit, false)]
        [InlineData(CardType.Credit, true)]
        public void Closed_Action5_Only_For_Credit(CardType cardType, bool expectAction5)
        {
            var card = new CardDetails("x", cardType, CardStatus.Closed, IsPinSet: false);
            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(expectAction5, actions.Contains(ActionType.ACTION5));

            Assert.Contains(ActionType.ACTION3, actions);
            Assert.Contains(ActionType.ACTION4, actions);
            Assert.Contains(ActionType.ACTION9, actions);
        }

        [Theory]
        [InlineData(CardStatus.Active, true)]
        [InlineData(CardStatus.Inactive, false)]
        [InlineData(CardStatus.Ordered, false)]
        [InlineData(CardStatus.Blocked, false)]
        public void Action1_Only_When_Active(CardStatus status, bool expectAction1)
        {
            var card = new CardDetails("x", CardType.Debit, status, IsPinSet: true);
            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(expectAction1, actions.Contains(ActionType.ACTION1));
        }

        [Theory]
        [InlineData(true, true, false)]   // with PIN: action 6
        [InlineData(false, false, true)]  // without PIN: action 7
        public void Active_Pin_Rules_For_Action6_And_7(bool isPinSet, bool expectAction6, bool expectAction7)
        {
            var card = new CardDetails("x", CardType.Debit, CardStatus.Active, isPinSet);
            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(expectAction6, actions.Contains(ActionType.ACTION6));
            Assert.Equal(expectAction7, actions.Contains(ActionType.ACTION7));
        }

        [Theory]
        [InlineData(true, true, true)]    // Blocked + PIN: action 6 and 7
        [InlineData(false, false, false)] // Blocked without PIN: missing action 6 and 7
        public void Blocked_Pin_Rules_For_Action6_And_7(bool isPinSet, bool expectAction6, bool expectAction7)
        {
            var card = new CardDetails("x", CardType.Credit, CardStatus.Blocked, isPinSet);
            var actions = _resolver.GetAllowedActions(card);

            Assert.Equal(expectAction6, actions.Contains(ActionType.ACTION6));
            Assert.Equal(expectAction7, actions.Contains(ActionType.ACTION7));
        }

        #endregion Theories
    }
}