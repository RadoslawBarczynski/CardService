using CardService.Api.Models.Enums;

namespace CardService.Api.Models
{
    public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
}
