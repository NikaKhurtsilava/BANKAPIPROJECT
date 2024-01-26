namespace BANKAPI.Model
{
    public class WinResponse
    {
        public int? TransactionId { get; set; }
        public decimal CurrentBalance { get; set; }
        public int StatusCode { get; set; }
    }
}
