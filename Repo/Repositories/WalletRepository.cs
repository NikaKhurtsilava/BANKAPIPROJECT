using BANKAPI.Model;
using Dapper;
using System.Data;

namespace BANKAPI.Repo.Repositories
{
    public class WalletRepository
    {
        private readonly IDbConnection _dbConnection;

        public WalletRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public AuthResponse AuthenticateUser(string publicToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@PublicToken", Guid.Parse(publicToken), DbType.Guid, ParameterDirection.Input);
            parameters.Add("@PrivateToken", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("@IsPublicTokenExpired", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            _dbConnection.Execute("AuthProcedure", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 401:
                case 404:
                case 500:
                    return new AuthResponse
                    {
                        PrivateToken = Guid.Empty,
                        StatusCode = errorCode,
                    };
                default:
                    var privateToken = parameters.Get<Guid>("@PrivateToken");

                    return new AuthResponse
                    {
                        PrivateToken = privateToken,
                        StatusCode = errorCode,
                    };
            }
        }
        public BalanceResponse GetBalance(Guid privateToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PrivateToken", privateToken, DbType.Guid, ParameterDirection.Input);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _dbConnection.Execute("GetBalanceProcedure", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 401:
                case 404:
                case 500:
                    return new BalanceResponse
                    {
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    var currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new BalanceResponse
                    {
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }
        }
        public PlayerInfoResponse GetPlayerInfo(Guid privateToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PrivateToken", privateToken, DbType.Guid, ParameterDirection.Input);
            parameters.Add("@UserId", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@UserName", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _dbConnection.Execute("GetPlayerInfoProcedure", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 401:
                case 500:
                    return new PlayerInfoResponse
                    {
                        UserId = null,
                        UserName = null,
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    var userId = parameters.Get<string>("@UserId");
                    var userName = parameters.Get<string>("@UserName");
                    var currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new PlayerInfoResponse
                    {
                        UserId = userId,
                        UserName = userName,
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }

        }
        public BetResponse AddBetTransaction(int remoteTransactionId, decimal amount, Guid privateToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RemoteTransactionId", remoteTransactionId);
            parameters.Add("@Amount", amount, DbType.Decimal, ParameterDirection.Input, precision: 18, scale: 2);
            parameters.Add("@PrivateToken", privateToken, DbType.Guid, ParameterDirection.Input);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _dbConnection.Execute("AddBetTransaction", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 201:
                case 401:
                case 404:
                case 500:
                    return new BetResponse
                    {
                        TransactionId = null,
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    var transactionId = parameters.Get<int>("@RemoteTransactionId");
                    var currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new BetResponse
                    {
                        TransactionId = transactionId,
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }
        }
        public WinResponse AddWinTransaction(int remoteTransactionId, decimal amount, Guid privateToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RemoteTransactionId", remoteTransactionId);
            parameters.Add("@Amount", amount);
            parameters.Add("@PrivateToken", privateToken);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);

            _dbConnection.Execute("AddWinTransaction", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 201:
                case 401:
                case 404:
                case 500:
                    return new WinResponse
                    {
                        TransactionId = null,
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    int? transactionId = null;
                    if (errorCode == 200)
                    {
                        transactionId = parameters.Get<int>("@RemoteTransactionId");
                    }

                    decimal currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new WinResponse
                    {
                        TransactionId = transactionId,
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }
        }
        public CancelBetResponse CancelBetTransaction(Guid privateToken, decimal amount, int remoteTransactionId, int betTransactionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RemoteTransactionId", remoteTransactionId);
            parameters.Add("@Amount", amount);
            parameters.Add("@PrivateToken", privateToken);
            parameters.Add("@BetTransactionId", betTransactionId);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);

            _dbConnection.Execute("CancelBetTransaction", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 201:
                case 401:
                case 404:
                case 500:
                    return new CancelBetResponse
                    {
                        TransactionId = null,
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    int? transactionId = null;
                    if (errorCode == 200)
                    {
                        transactionId = parameters.Get<int>("@RemoteTransactionId");
                    }

                    decimal currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new CancelBetResponse
                    {
                        TransactionId = transactionId,
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }
        }
        public ChangeWinResponse ChangeWinTransaction(Guid privateToken, decimal amount, int remoteTransactionId, int previousRemoteTransactionId, decimal previousAmount)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RemoteTransactionId", remoteTransactionId);
            parameters.Add("@Amount", amount);
            parameters.Add("@PrivateToken", privateToken);
            parameters.Add("@PreviousRemoteTransactionId", previousRemoteTransactionId);
            parameters.Add("@PreviousAmount", previousAmount);
            parameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@CurrentBalance", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 18, scale: 2);

            _dbConnection.Execute("ChangeWinTransaction", parameters, commandType: CommandType.StoredProcedure);

            int errorCode = parameters.Get<int>("@ErrorCode");

            switch (errorCode)
            {
                case 201:
                case 401:
                case 500:   
                    return new ChangeWinResponse
                    {
                        TransactionId = null,
                        CurrentBalance = 0,
                        StatusCode = errorCode,
                    };
                default:
                    int? transactionId = null;
                    if (errorCode == 200)
                    {
                        transactionId = parameters.Get<int>("@RemoteTransactionId");
                    }

                    decimal currentBalance = parameters.Get<decimal>("@CurrentBalance");

                    return new ChangeWinResponse
                    {
                        TransactionId = transactionId,
                        CurrentBalance = currentBalance,
                        StatusCode = errorCode,
                    };
            }
        }


    }
}
