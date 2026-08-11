using CardService.Domain;
using CardService.Domain.Enums;

namespace CardService.Domain.Services
{
    public sealed class AllowedActionsResolver : IAllowedActionsResolver
    {
        //get all actions from enum
        private static readonly ActionType[] AllActions = Enum.GetValues<ActionType>()
                                   .OrderBy(action => (int)action)
                                   .ToArray();

        public IReadOnlyCollection<ActionType> GetAllowedActions(CardDetails card)
        {
            if (card is null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            var result = new List<ActionType>();

            foreach (var action in AllActions)
            {
                if (IsAllowed(card, action))
                    result.Add(action);
            }
            return result;
        }

        private static bool IsAllowed(CardDetails card, ActionType action)
        {
            if (!IsAllowedForType(card.CardType, action))
            {
                return false;
            }

            return IsAllowedForCardStatus(action, card.CardStatus, card.IsPinSet);
        }

        private static bool IsAllowedForType(CardType cardType, ActionType action)
        {
            if (action == ActionType.ACTION5)
                return cardType == CardType.Credit;

            return true;
        }

        private static bool IsAllowedForCardStatus(ActionType action, CardStatus status, bool isPinSet)
        {
            //it can be done more robust, but in case of recruitment process assingment, I simplified it
            //for a larger/ changing rule set this could be a declarative matrix
            //action -> allowed statuses + pin policy or rules loaded from config
            switch (action)
            {
                case ActionType.ACTION1:
                    return status == CardStatus.Active;

                case ActionType.ACTION2:
                    return status == CardStatus.Inactive;

                case ActionType.ACTION3:
                case ActionType.ACTION4:
                case ActionType.ACTION5:
                case ActionType.ACTION9:
                    return true;

                case ActionType.ACTION6:
                    if (status == CardStatus.Ordered
                        || status == CardStatus.Inactive
                        || status == CardStatus.Active
                        || status == CardStatus.Blocked)
                    {
                        return isPinSet;
                    }

                    return false;

                case ActionType.ACTION7:
                    if (status == CardStatus.Ordered
                        || status == CardStatus.Inactive
                        || status == CardStatus.Active)
                    {
                        return !isPinSet;
                    }
                    if (status == CardStatus.Blocked)
                        return isPinSet;

                    return false;

                case ActionType.ACTION8:
                    return status == CardStatus.Ordered
                        || status == CardStatus.Inactive
                        || status == CardStatus.Active
                        || status == CardStatus.Blocked;

                case ActionType.ACTION10:
                case ActionType.ACTION12:
                case ActionType.ACTION13:
                    return status == CardStatus.Ordered
                        || status == CardStatus.Inactive
                        || status == CardStatus.Active;

                case ActionType.ACTION11:
                    return status == CardStatus.Inactive
                        || status == CardStatus.Active;

                default:
                    return false;
            }
        }
    }
}