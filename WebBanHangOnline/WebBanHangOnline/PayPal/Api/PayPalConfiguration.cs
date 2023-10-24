namespace PayPal.Api
{
    internal class PayPalConfiguration : APIContext
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
}