using CardService.Domain;
using CardService.Domain.Enums;

namespace CardService.Domain.Services
{
    public interface IAllowedActionsResolver
    {
        IReadOnlyCollection<ActionType> GetAllowedActions(CardDetails card);
    }
}