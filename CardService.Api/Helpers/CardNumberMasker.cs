namespace CardService.Api.Helpers
{
    public static class CardNumberMasker
    {
        public static string Mask(string? cardNumber)
        {
            if (String.IsNullOrEmpty(cardNumber))
            {
                return "****";
            }

            if(cardNumber.Length <= 4)
            {
                return new string('*', cardNumber.Length);
            }

            string mask = new string('*', cardNumber.Length - 4);
            string lastFour = cardNumber.Substring(cardNumber.Length - 4);

            return mask + lastFour;
        }
    }
}
