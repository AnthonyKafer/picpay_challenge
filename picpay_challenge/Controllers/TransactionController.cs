using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services;
using picpay_challenge.Helpers;
using picpay_challenge.Middleware;
using System.Net;
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
        /// <param name="query">Parâmetros possíveis</param>
        /// <returns>Retorna a lista de todas as transações criadas.</returns>
        /// <response code="401">Caso o usuário não seja um administrador.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult<List<ResponseTransactionDTO>> GetTransactions([FromServices] UserService userService, [FromQuery] TransactionQuery query)
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            string? roleClaim = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == null)
                return Unauthorized("Login to use the system.");


            BaseUser.Roles role = Enum.Parse<BaseUser.Roles>(roleClaim);

            if (role != BaseUser.Roles.Admin) return Unauthorized("You can only see details of your own account");

            return Ok(_transactionService.FindMany(query));
        }

        /// <summary>
        /// Busca transações iniciadas pelo usuário.
        /// </summary>
        /// <returns>Retorna uma lista de transações do usuário.</returns>
        [HttpGet("user/me")]
        [ProducesResponseType(typeof(List<ResponseTransactionDTO?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        public ActionResult<List<ResponseTransactionDTO?>?> GetUserTransactions()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (currentUserId == null) return Unauthorized("Login to use the system.");

            List<ResponseTransactionDTO?> transactionsList = _transactionService.GetUserTransactions(currentUserId);

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
            ClaimsPrincipal currentUser = HttpContext.User;

            string? roleClaim = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == null)
                return Unauthorized("Login to use the system.");

            BaseUser.Roles role = Enum.Parse<BaseUser.Roles>(roleClaim);

            if (role != BaseUser.Roles.User) throw new HttpException(HttpStatusCode.Unauthorized, "Only regular users can pay transfers");
            var payment = await _transactionService.Create(userService, payload);
            if (payment == null) return BadRequest(payment);
            return Ok(payment);
        }


    }
}
