using BANKAPI.Model;
using Dapper;
using System.Data;

namespace BANKAPI.Repo.Services
{
    public class TokenService
    {
        private readonly IDbConnection _dbConnection;

        public TokenService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public static string GeneratePrivateToken()
        {
            return Guid.NewGuid().ToString("N"); 
        }

        public string GetUserIdByPrivateToken(Guid privateToken)
        {
            const string sql = "SELECT UserId FROM Tokens WHERE PrivateToken = @PrivateToken";
            return _dbConnection.QueryFirstOrDefault<string>(sql, new { PrivateToken = privateToken });
        }
        public void AddBetTransaction(int remoteTransactionId, decimal amount, Guid privateToken)
        {
            var parameters = new
            {
                RemoteTransactionId = remoteTransactionId,
                Amount = amount,
                PrivateToken = privateToken
            };

            _dbConnection.Execute("AddBetTransaction", parameters, commandType: CommandType.StoredProcedure);
        }
        public void AddWinTransaction(int remoteTransactionId, decimal amount, Guid privateToken)
        {
            var parameters = new
            {
                RemoteTransactionId = remoteTransactionId,
                Amount = amount,
                PrivateToken = privateToken
            };

            _dbConnection.Execute("AddWinTransaction", parameters, commandType: CommandType.StoredProcedure);
        }
        public void CancelBetTransaction(Guid privateToken, decimal amount, int remoteTransactionId, int betTransactionId)
        {
            var parameters = new
            {
                PrivateToken = privateToken,
                Amount = amount,
                RemoteTransactionId = remoteTransactionId,
                BetTransactionId = betTransactionId
            };

            _dbConnection.Execute("CancelBetTransaction", parameters, commandType: CommandType.StoredProcedure);
        }

        public void ChangeWinTransaction(Guid privateToken, decimal amount, int remoteTransactionId, int previousRemoteTransactionId, decimal previousAmount)
        {
            var parameters = new
            {
                PrivateToken = privateToken,
                Amount = amount,
                RemoteTransactionId = remoteTransactionId,
                PreviousRemoteTransactionId = previousRemoteTransactionId,
                PreviousAmount = previousAmount
            };

            _dbConnection.Execute("ChangeWinTransaction", parameters, commandType: CommandType.StoredProcedure);
        }
        public bool IsTokenValid(string token)
        {
            var tokenInfo = GetTokenInfoFromDatabase(token);

            return tokenInfo != null && tokenInfo.TokenExpiryDate > DateTime.Now;
        }
        private TokenInfo GetTokenInfoFromDatabase(string token)
        {
            const string sql = "SELECT * FROM Tokens WHERE PublicToken = @PublicToken OR PrivateToken = @PrivateToken";
            return _dbConnection.QueryFirstOrDefault<TokenInfo>(sql, new { PublicToken = token, PrivateToken = token });
        }
        private void UpdatePrivateTokenExpiredStatus(Guid privateToken)
        {
            const string updateSql = "UPDATE Tokens SET PrivateTokenExpired = 1 WHERE PrivateToken = @PrivateToken";
            _dbConnection.Execute(updateSql, new { PrivateToken = privateToken });
        }

    }
}
