using BANKAPI.Model;
using BANKAPI.Repo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace BANKAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : Controller
    {
        private readonly WalletRepository _walletRepository;
        private readonly IDbConnection _dbConnection;

        public WalletController(WalletRepository walletRepository,IDbConnection dbConnection)
        {
            _walletRepository = walletRepository;
            _dbConnection = dbConnection;
        }

        [HttpPost("Auth")]
        public ActionResult<object> Auth([FromQuery] string publicToken)
        {
            try
            {
                var authResponse = _walletRepository.AuthenticateUser(publicToken);

                switch (authResponse.StatusCode)
                {
                    case 401:
                    case 404:
                    case 500:
                        var errorModel = new ErrorModel { StatusCode = authResponse.StatusCode };
                        return StatusCode(200, errorModel);
                    default:
                        var responseData = new { PrivateToken = authResponse.PrivateToken };
                        var successModel = new { StatusCode = authResponse.StatusCode, Data = responseData };
                        return Ok(successModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new { StatusCode = 500 });
            }
        }

        [HttpPost("GetBalance")]
        public ActionResult<object> GetBalance([FromQuery] Guid privateToken)
        {
            try
            {
                var response = _walletRepository.GetBalance(privateToken);

                switch (response.StatusCode)
                {
                    case 401:
                    case 404:
                    case 500:
                        var errorModel = new ErrorModel { StatusCode = response.StatusCode };
                        return StatusCode(200, errorModel);
                    default:
                        return Ok(new { StatusCode = 200, Data = new { Balance = response.CurrentBalance } });
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new { StatusCode = 500 });
            }
        }

        [HttpPost("GetPlayerInfo")]
        public ActionResult<object> GetPlayerInfo([FromQuery] Guid privateToken)
        {
            try
            {
                var response = _walletRepository.GetPlayerInfo(privateToken);

                switch (response.StatusCode)
                {
                    case 401:
                    case 404:
                    case 500:
                        var errorModel = new ErrorModel { StatusCode = response.StatusCode };
                        return StatusCode(200, errorModel);
                    default:
                        var responseData = new
                        {
                            UserId = response.UserId,
                            UserName = response.UserName,
                            CurrentBalance = response.CurrentBalance
                        };
                        var successModel = new { StatusCode = response.StatusCode, Data = responseData };
                        return Ok(successModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new { StatusCode = 500 });
            }
        }

        [HttpPost("Bet")]
        public ActionResult<object> Bet([FromQuery] int remoteTransactionId, [FromQuery] decimal amount, [FromQuery] Guid privateToken)
        {
            try
            {
                var response = _walletRepository.AddBetTransaction(remoteTransactionId, amount, privateToken);

                switch (response.StatusCode)
                {
                    case 201:
                    case 401:
                    case 404:
                    case 500:
                        var errorModel = new ErrorModel { StatusCode = response.StatusCode };
                        return StatusCode(200, errorModel);
                    default:
                        var responseData = new
                        {
                            TransactionId = response.TransactionId,
                            CurrentBalance = response.CurrentBalance
                        };
                        var successModel = new { StatusCode = response.StatusCode, Data = responseData };
                        return Ok(successModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new { StatusCode = 500 });
            }
        }
        [HttpPost("Win")]
        public ActionResult<object> Win([FromQuery] int remoteTransactionId, [FromQuery] decimal amount, [FromQuery] Guid privateToken)
        {
            try
            {
                var winResponse = _walletRepository.AddWinTransaction(remoteTransactionId, amount, privateToken);
                var errorModel = new ErrorModel { StatusCode = winResponse.StatusCode };
                switch (winResponse.StatusCode)
                {
                    case 200:
                        var responseBody = new
                        {
                            Data = new
                            {
                                TransactionId = winResponse.TransactionId,
                                CurrentBalance = winResponse.CurrentBalance
                            },
                            StatusCode = 200
                        };
                        return Ok(responseBody);
                    case 201:
                    case 401:
                    case 404:
                    case 500:
                        return StatusCode(200, errorModel);
                    default:
                        return StatusCode(200, errorModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new ErrorModel { StatusCode = 500 });
            }
        }

        [HttpPost("CancelBet")]
        public IActionResult CancelBet([FromQuery] Guid privateToken, [FromQuery] decimal amount, [FromQuery] int remoteTransactionId, [FromQuery] int betTransactionId)
        {
            try
            {
                var cancelBetResponse = _walletRepository.CancelBetTransaction(privateToken, amount, remoteTransactionId, betTransactionId);
                var errorModel = new ErrorModel { StatusCode = cancelBetResponse.StatusCode };
                switch (cancelBetResponse.StatusCode)
                {
                    case 200:
                        var responseBody = new
                        {
                            StatusCode = 200,
                            Data = new
                            {
                                TransactionId = cancelBetResponse.TransactionId,
                                CurrentBalance = cancelBetResponse.CurrentBalance
                            }
                        };
                        return Ok(responseBody);
                    case 201:
                    case 401:
                    case 404:
                    case 500:
                        return StatusCode(200, errorModel);
                    default:
                        return StatusCode(200, errorModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new ErrorModel { StatusCode = 500 });
            }
        }

        [HttpPost("ChangeWin")]
        public IActionResult ChangeWin([FromQuery] Guid privateToken, [FromQuery] decimal amount, [FromQuery] int remoteTransactionId, [FromQuery] int previousRemoteTransactionId, [FromQuery] decimal previousAmount)
        {
            try
            {
                var changeWinResponse = _walletRepository.ChangeWinTransaction(privateToken, amount, remoteTransactionId, previousRemoteTransactionId, previousAmount);
                var errorModel = new ErrorModel { StatusCode = changeWinResponse.StatusCode };
                switch (changeWinResponse.StatusCode)
                {
                    case 200:
                        var responseBody = new
                        {
                            StatusCode = 200,
                            Data = new
                            {
                                TransactionId = changeWinResponse.TransactionId,
                                CurrentBalance = changeWinResponse.CurrentBalance
                            }
                        };
                        return Ok(responseBody);
                    case 201:
                    case 401:
                    case 404:
                    case 500:
                        return StatusCode(200, errorModel);
                    default:
                        return StatusCode(200, errorModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(200, new ErrorModel { StatusCode = 500 });
            }
        }

    }
}
