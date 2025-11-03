using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services;
using picpay_challenge.Helpers;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace picpay_challenge.Controllers
{
    [ApiController]
    [Route("/")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("create-account")]
        public ActionResult<ResponseUserDTO> CreateUser([FromBody] CreateUserDTO UserPayload)
        {
            bool isValidEmail = Validator.IsValidEmail(UserPayload.Email);
            bool isValidCPF = Validator.IsValidCPF(UserPayload.CPF);
            bool isValidCNPJ = UserPayload.CNPJ == "" || UserPayload.CNPJ == null ? true : Validator.IsValidCNPJ(UserPayload.CNPJ);

            bool isValidBalance = UserPayload.Balance > 0;

            if (
                !isValidEmail ||
                !isValidCPF ||
                !isValidCNPJ ||
                !isValidBalance
                ) return BadRequest("Invalid fields");

            var newUserRegistry = _userService.Create(UserPayload);

            return Ok(newUserRegistry);
        }
        [HttpPost("create-admin")]
        public ActionResult<ResponseUserDTO> CreateAdmin([FromBody] CreateUserDTO UserPayload)
        {
            bool isValidCPF = Validator.IsValidCPF(UserPayload.CPF);
            if (!isValidCPF) return BadRequest("Invalid fields");
            var newUserRegistry = _userService.CreateAdmin(UserPayload);

            return Ok(newUserRegistry);
        }
        [HttpPost("login")]
        public ActionResult Login([FromServices] AuthService authService, [FromBody] LoginUserDTO loginPayload)
        {
            if (!_userService.ValidadateUserCredentials(loginPayload)) return Unauthorized();

            var token = authService.GenerateToken(loginPayload.Email);
            return Ok(new { token });
        }


        [Authorize]
        [HttpGet("/user/{id}")]
        public ActionResult<ResponseUserDTO> GetUser(int id)
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.FindById(id);

            if (user == null) return NotFound();
            if (user.Email != currentUserEmail && user.Role != BaseUser.Roles.Admin) return Unauthorized("You can only see details of your own account");
            return Ok(user);

        }
        [Authorize]
        [HttpDelete("/user/delete/{id}")]
        public ActionResult<ResponseUserDTO> DeleteUser(string id)
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.FindById(int.Parse(id));
            if (user == null) return NotFound();
            if (user.Email != currentUserEmail && user.Role != BaseUser.Roles.Admin) return Unauthorized("You can only delete your own account");
            var res = _userService.Delete(int.Parse(id));

            return Ok(res);

        }
    }
}
