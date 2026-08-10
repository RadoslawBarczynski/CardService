namespace CardService.Api.Models
{
    public class CardServiceOptions
    {
        public const string SectionName = "CardService";
        public int ExternalCallDelayMs { get; set; } = 1000;
    }
}
