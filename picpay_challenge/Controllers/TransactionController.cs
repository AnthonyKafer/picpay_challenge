using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services;
using picpay_challenge.Middleware;
using System.Security.Claims;

namespace picpay_challenge.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        /// <summary>
        /// Lista todas as transações criadas, só deve ser permitido à administradores.
        /// </summary>
        /// <returns>Retorna a lista de todas as transações criadas.</returns>
        /// <response code="401">Caso o usuário não seja um administrador.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult<List<ResponseTransactionDTO>> GetTransactions([FromServices] UserService userService)
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            string email = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            var user = userService.FindByEmail(email);

            if (user.Email != email && user.Role != BaseUser.Roles.Admin) return Unauthorized("You can only see details of your own account");

            return Ok(_transactionService.FindMany());
        }
        /// <summary>
        /// Busca transação por id.
        /// </summary>
        /// <param name="transactionId">Id da transação.</param>
        /// <param name="userService">Serviço de clientes para validar o usuário.</param>
        /// <returns>Retorna a transação referida pelo id.</returns>
        /// <response code="401">Caso o usuário não seja o autor da transação.</response>
        /// <response code="404">Caso a transação não exista.</response>
        /// <response code="401">Caso o usuário não seja um administrador.</response>
        [HttpGet("{transactionId}")]
        [ProducesResponseType(typeof(ResponseTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        public ActionResult<ResponseTransactionDTO> GetTransactionById([FromServices] UserService userService, int transactionId)
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            var email = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null) return Unauthorized("Login to use the system");

            var user = userService.FindByEmail(email);

            if (user == null) return Unauthorized("You can only see details of your own account");

            var transaction = _transactionService.FindById(transactionId, user.Id);
            return Ok(transaction);
        }

        /// <summary>
        /// Busca transações iniciadas pelo usuário.
        /// </summary>
        /// <param name="userId">O ID do usuário pagante.</param>
        /// <returns>Retorna uma lista de transações do usuário.</returns>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(List<ResponseTransactionDTO?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        public ActionResult<List<ResponseTransactionDTO?>?> GetUserTransactions([FromServices] UserService userService, [FromRoute] int userId)
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            string email = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            var user = userService.FindById(userId);

            List<ResponseTransactionDTO?> transactionsList = _transactionService.GetUserTransactions(userService, userId, email);

            if (transactionsList.Count == 0) return NoContent();
            return Ok(transactionsList);
        }

        /// <summary>
        /// Faz uma transação.
        /// </summary>
        /// <param name="payload">Informações relativas ao pagamento, Id do pagante e recebedor, além do valor.</param>
        /// <returns>Retorna a transação criada.</returns>
        /// <response code="400">Caso o payload tenha algum problema.</response>
        /// <response code="401">Caso o usuário não tenha saldo.</response>
        /// <response code="404">Caso o pagante ou recebor não existam.</response>
        /// <response code="401">Caso o pagante não seja um usuário padrão.</response>
        /// <response code="401">Caso o integrador não autorize a transação.</response>
        [HttpPost("make-payment")]
        [ProducesResponseType(typeof(ResponseTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 400)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public async Task<ActionResult<ResponseTransactionDTO?>> MakePayment([FromServices] UserService userService, [FromBody] CreateTransactionDTO payload)
        {

            var payment = await _transactionService.Create(userService, payload);
            if (payment == null) return BadRequest(payment);
            return Ok(payment);
        }


    }
}
