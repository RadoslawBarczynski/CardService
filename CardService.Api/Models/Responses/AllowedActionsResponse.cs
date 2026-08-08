using CardService.Api.Models.Enums;

namespace CardService.Api.Models.Responses
{
    public sealed class AllowedActionsResponse
    {
        public required IReadOnlyCollection<ActionType> Actions { get; init; }
    }
}
