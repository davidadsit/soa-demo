namespace PizzaCreationService
{
    public class CouponIssuedMessage
    {
        public string CorrelationId { get; set; }
        public string Coupon { get; set; }
    }
}