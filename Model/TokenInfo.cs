namespace BANKAPI.Model
{
    public class TokenInfo
    {
        public string UserId { get; set; }
        public Guid PublicToken { get; set; }
        public int PublicTokenExpired { get; set; }
        public Guid PrivateToken { get; set; }
        public int PrivateTokenExpired { get; set; }
        public DateTime TokenExpiryDate { get; set; }
        public bool IsPublicTokenExpired()
        {
            return PublicTokenExpired == 1;
        }

        public bool IsPrivateTokenExpired()
        {
            return PrivateTokenExpired == 1;
        }
    }
}
