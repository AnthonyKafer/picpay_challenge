using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Filter;
using picpay_challenge.Domain.Models.Transaction;
using picpay_challenge.Domain.Models.User;
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
        [RoleBasedFilter([BaseUser.Roles.Admin])]
        [HttpGet]
        [ProducesResponseType(typeof(ResponseListTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public async Task<ActionResult<ResponseListTransactionDTO>> GetTransactions([FromQuery] TransactionQuery query)
        {
            var transactions = await _transactionService.FindMany(query);
            return Ok(transactions);
        }

        /// <summary>
        /// Busca transações iniciadas pelo usuário.
        /// </summary>
        /// <returns>Retorna uma lista de transações do usuário.</returns>
        [HttpGet("user/me")]
        [ProducesResponseType(typeof(ResponseListTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        public async Task<ActionResult<ResponseListTransactionDTO>> GetUserTransactions([FromQuery] TransactionQuery query)
        {
            ResponseListTransactionDTO transactionsList = await _transactionService
                .GetUserTransactions(query, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
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
        [RoleBasedFilter([BaseUser.Roles.User])]
        [HttpPost("make-payment")]
        [ProducesResponseType(typeof(ResponseSingleTransactionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 400)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public async Task<ActionResult<ResponseSingleTransactionDTO?>> MakePayment([FromServices] UserService userService, [FromBody] CreateTransactionDTO payload)
        {
            var payment = await _transactionService.Create(userService, payload, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
            if (payment == null) return BadRequest(payment);
            return Ok(payment);
        }


    }
}
