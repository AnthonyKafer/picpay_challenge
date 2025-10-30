namespace picpay_challenge.Domain.DTOs.PaymentExternalAuthorizorDTOs
{
    public class PaymentAuthorizationDTO
    {
        public class PaymentAuthorizationData
        {
            public bool authorization { get; set; }
        }
        public enum statusTypes
        {
            success,
            fail,
        }
        public statusTypes Status;
        public PaymentAuthorizationData Data { get; set; } = new PaymentAuthorizationData();
    }
}
