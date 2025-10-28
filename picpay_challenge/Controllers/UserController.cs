using Microsoft.AspNetCore.Mvc;
using picpay_challenge.DTOs.UserDTOs;
using PicPayChallenge.Services;
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
        public IActionResult CreateUser([FromBody] CreateUserPayloadDTO UserPayload)
        {
            //var isValidEmail = Regex.IsMatch(UserPayload.Email, @"/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/");
            //var isValidCPF = Regex.IsMatch(UserPayload.CPF, @"[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}\-?[0-9]{2}");
            //var isValidCNPJ = Regex.IsMatch(UserPayload.CNPJ, @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2}");
            //var isValidBalance = UserPayload.Balance < 0;

            if (!ModelState.IsValid) return BadRequest("Invalid Fields");
            var newUserRegistry = _userService.CreateUser(UserPayload);

            return Ok(newUserRegistry);
        }

        [HttpGet("/user/{id}")]
        public IActionResult GetUser(string id)
        {
            var user = _userService.GetById(int.Parse(id));
            if (user == null) return NotFound();
            return Ok(user);

        }
        [HttpDelete("/user/delete/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _userService.Delete(int.Parse(id));
            if (!user) return NotFound();
            return Ok(new { Message = "Sucessfully deleted!" });

        }
    }
}
