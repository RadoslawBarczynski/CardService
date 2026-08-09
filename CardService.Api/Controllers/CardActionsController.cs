using CardService.Api.Helpers;
using CardService.Api.Models.Responses;
using CardService.Api.Services;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardService.Api.Controllers
{
    [ApiController]
    [Route("api/cards")]
    public class CardActionsController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly IAllowedActionsResolver _actionsResolver;
        private readonly ILogger<CardActionsController> _logger;

        public CardActionsController(ICardService cardService, IAllowedActionsResolver actionsResolver, ILogger<CardActionsController> logger)
        {
            _cardService = cardService;
            _actionsResolver = actionsResolver;
            _logger = logger;
        }

        [HttpGet("{userId}/{cardNumber}/actions")]
        public async Task<IActionResult> GetAllowedActions(string userId, string cardNumber, CancellationToken cancellationToken)
        {
            var maskedCardNumber = CardNumberMasker.Mask(cardNumber);

            _logger.LogInformation("Resolving allowed actions for user {UserId}, card {MaskedCardNumber}", userId, maskedCardNumber);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(cardNumber))
            {
                return BadRequest("userId and cardNumber are required");
            }

            var card = await _cardService.GetCardDetails(userId, cardNumber, cancellationToken);

            if (card is null)
            {
                _logger.LogWarning("Card not found for user {UserId}, card {MaskedCardNumber}", userId, maskedCardNumber);

                return NotFound($"Card '{cardNumber}' for user '{userId}' not found");
            }

            var actions = _actionsResolver.GetAllowedActions(card);

            _logger.LogInformation("Resolved {ActionCount} actions for user {UserId}, card {MaskedCardNumber}, type {CardType}, status {CardStatus}",
                actions.Count,
                userId,
                maskedCardNumber,
                card.CardType,
                card.CardStatus);

            return Ok(new AllowedActionsResponse { Actions = actions});
        }
    }
}
