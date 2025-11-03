using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using picpay_challenge.Domain.DTOs.TransactionsDTOs;
using picpay_challenge.Domain.DTOs.UserDTOs;
using picpay_challenge.Domain.Exceptions;
using picpay_challenge.Domain.Models;
using picpay_challenge.Domain.Services;
using picpay_challenge.Helpers;
using picpay_challenge.Middleware;
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
        /// <summary>
        /// Cria uma conta.
        /// </summary>
        /// <param name="UserPayload">Informações relativas ao Usuário</param>
        /// <returns>Retorna o usuário criado.</returns>
        /// <response code="400">Caso o payload tenha algum problema.</response>
        /// <response code="409">Caso alguma das credenciais usadas já tiver sido usada em outro registro de usuário.</response>
        /// <response code="500">Caso ocorra algum erro ao salvar.</response>
        [HttpPost("create-account")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 400)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 409)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 500)]
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
        /// <summary>
        /// Cria um usuário administrador.
        /// </summary>
        /// <param name="UserPayload">Informações relativas ao administrador.</param>
        /// <returns>Retorna o administrador criado.</returns>
        /// <response code="400">Caso o CPF não seja válido.</response>
        /// <response code="409">Caso alguma das credenciais usadas já tiver sido usada em outro registro de usuário.</response>
        /// <response code="500">Caso ocorra algum erro ao salvar.</response>
        [HttpPost("create-admin")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 400)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 409)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 500)]
        public ActionResult<ResponseUserDTO> CreateAdmin([FromBody] CreateUserDTO UserPayload)
        {
            bool isValidCPF = Validator.IsValidCPF(UserPayload.CPF);
            if (!isValidCPF) return BadRequest("Invalid fields");
            var newUserRegistry = _userService.CreateAdmin(UserPayload);

            return Ok(newUserRegistry);
        }

        /// <summary>
        /// Faz o login de um usuário.
        /// </summary>
        /// <param name="loginPayload">Informações relativas ao login.</param>
        /// <returns>Retorna o token JWT Bearer.</returns>
        /// <response code="401">Caso os dados sejão inválidos.</response>
        /// <response code="404">Caso o usuário não exista.</response>

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult Login([FromServices] AuthService authService, [FromBody] LoginUserDTO loginPayload)
        {
            var user = _userService.ValidadateUserCredentials(loginPayload);
            if (user == null) return Unauthorized();

            var token = authService.GenerateToken(loginPayload.Email, user.Role, user.Id);
            return Ok(new { token });
        }

        /// <summary>
        /// Busca o registros de usuários - apenas administradores podem acessar.
        /// </summary>
        /// <param name="query">Parâmetros de filtros.</param>
        /// <returns>Retorna o usuário.</returns>
        /// <response code="401">Caso o usuário não seja um administrador.</response>
        /// <response code="404">Caso o usuário não exista.</response>
        [Authorize]
        [HttpGet("/users")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult<ResponseUserDTO> GetUsers([FromQuery] UserQuery query)
        {
            ClaimsPrincipal currentUser = HttpContext.User;
            string? roleClaim = currentUser.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim == null) return Unauthorized("Login to use the system.");

            BaseUser.Roles role = Enum.Parse<BaseUser.Roles>(roleClaim);
            if (role != BaseUser.Roles.Admin) return Unauthorized("You can only see details of your own account");

            return Ok(_userService.FindMany(query));

        }

        /// <summary>
        /// Busca o registro de um usuário.
        /// </summary>
        /// <returns>Retorna o usuário.</returns>
        /// <response code="401">Caso o usuário não seja um administrador e esteja vendo um registro que não é dele.</response>
        /// <response code="404">Caso o usuário não exista.</response>
        [Authorize]
        [HttpGet("/user/me")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult<ResponseUserDTO> GetUser()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (currentUserId == null) return Unauthorized("Login to use the system");
            return Ok(_userService.FindById(currentUserId));

        }
        /// <summary>
        /// Exclui o registro de um usuário - soft delete.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>Retorna o usuário.</returns>
        /// <response code="401">Caso o usuário esteja excluindo um registro que não é dele.</response>
        /// <response code="404">Caso o usuário não exista.</response>
        [Authorize]
        [HttpDelete("/user/delete")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 404)]
        [ProducesResponseType(typeof(DefaultErrorMessage), 401)]
        public ActionResult<ResponseUserDTO> DeleteUser()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (currentUserId == null) return Unauthorized("Login to use the system");

            var res = _userService.Delete(currentUserId);

            return Ok(res);

        }
    }
}
