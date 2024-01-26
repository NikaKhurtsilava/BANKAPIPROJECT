namespace BANKAPI.Model
{
    public class PlayerInfoResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public int StatusCode { get; set; }
    }
}
