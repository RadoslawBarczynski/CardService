using Domain.Enums;

namespace Domain.Services
{
    public interface IAllowedActionsResolver
    {
        IReadOnlyCollection<ActionType> GetAllowedActions(CardDetails card);
    }
}