using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services;
using System.Reflection.Metadata.Ecma335;

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
        /// XXXXXXXXXXXXXXXXX
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public ActionResult<List<Transaction>> GetTransactions([FromQuery] TransactionFilterQueryDto filter)
        {
            return Ok(_transactionService.FindMany());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTransactionById([FromRoute] int id)
        {
            return Ok(_transactionService.FindById(id));
        }

        [HttpPost("make-payment")]
        public ActionResult MakePayment([FromBody] CreateTransactionDTO payload)
        {
            var payer = userService.FindById(payload.PayerId);
            var payee = userService.FindById(payload.PayeeId);
            if (payer == null || payee == null) return BadRequest("Payer or payee not found");
            if (payer.UserType == BaseUser.UserTypes.Storekeeper) return BadRequest("You cannot make transfers from an storekeeper account");

            var payment = _transactionService.Create(payload);

        }

    }

    public class TransactionFilterQueryDto
    {
        public DateTime? MinCreatedAt { get; set; }
        public DateTime? MaxCreatedAt { get; set; }
        public int? PayerId { get; set; }
        public int? PayeeId { get; set; }
    }

    public class ErrorResponseDto
    {
        public string Message { get; set; }
    }
}
