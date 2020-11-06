using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestSolution.Api.Data;
using TestSolution.Api.Data.Entities;
using TestSolution.Api.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TestSolution.Api.Helpers;
using System.Linq;

namespace TestSolution.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MainController : Controller
    {
        private ApplicationDbContext _applicationDbContext = null;
        private IConfiguration _configuration = null;
        public MainController(ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Returns the successful response</response>
        /// <response code="400">If the request is not valid</response>        
        /// <response code="500">If there is an internal server error</response>
        [AllowAnonymous]
        [HttpPost("account/register")]
        [ProducesResponseType(typeof(LoginResponseModel), 200)]
        [ProducesResponseType(typeof(BadRequestResponseModel), 400)]
        [ProducesResponseType(typeof(InternalServerErrorResponseModel), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) ||
                    string.IsNullOrEmpty(model.FirstName) ||
                    string.IsNullOrEmpty(model.LastName) ||
                    string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.Password))
                {
                    var badRequestResponse = new BadRequestResponseModel()
                    {
                        Message = "All fields are required.",
                        Description = "All fields are required.",
                        Details = new List<string>()
                    };
                    return StatusCode(400, badRequestResponse);
                }

                var user = await _applicationDbContext.User.FirstOrDefaultAsync(x => x.Username.ToLower() == model.Username.ToLower());
                if (user != null)
                {
                    var badRequestResponse = new BadRequestResponseModel()
                    {
                        Message = "Username is taken. Choose another username.",
                        Description = "Username is taken. Choose another username.",
                        Details = new List<string>()
                    };
                    return StatusCode(400, badRequestResponse);
                }

                user = new User();
                user.Id = 0;
                user.Username = model.Username;
                user.Password = Utilities.GetSHA256(model.Password);
                user.Name = model.FirstName;
                user.Surname = model.Username;
                user.Email = model.Email;
                _applicationDbContext.Add(user);
                await _applicationDbContext.SaveChangesAsync();
                return StatusCode(200);
            }
            catch (Exception exception)
            {
                var errorResponse = new InternalServerErrorResponseModel();
                errorResponse.LogId = Guid.NewGuid().ToString();
                errorResponse.Message = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="model"></param>
        /// <response code="200">Returns the response model</response>
        /// <response code="400">If the request is not valid</response>        
        /// <response code="500">If there is an internal server error</response>
        [AllowAnonymous]
        [HttpPost("account/authenticate")]
        [ProducesResponseType(typeof(LoginResponseModel), 200)]
        [ProducesResponseType(typeof(InternalServerErrorResponseModel), 500)]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequestModel model)
        {
            try
            {
                var user = await _applicationDbContext.User.FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == Utilities.GetSHA256(model.Password));
                if (user != null)
                {
                    //SignIn https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/
                    var claimCollection = new List<Claim> { new Claim("Username", user.Username) };
                    var jwtKey = _configuration["JwtKey"];
                    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
                    var jwtIssuer = _configuration["JwtIssuer"];
                    var jwtSecurityToken = new JwtSecurityToken(jwtIssuer,
                        jwtIssuer,
                        claimCollection,
                        expires: DateTime.Now.AddMinutes(model.Expiration),
                        signingCredentials: signingCredentials);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    //Response
                    var responseModel = new LoginResponseModel();
                    responseModel.Success = true;
                    responseModel.Message = "Login completed successfully.";
                    responseModel.Token = tokenString;
                    return StatusCode(200, responseModel);
                }
                else
                {
                    var responseModel = new LoginResponseModel();
                    responseModel.Success = false;
                    responseModel.Message = "Invalid login attempt.";
                    responseModel.Token = "";
                    return StatusCode(200, responseModel);
                }
            }
            catch (Exception exception)
            {
                var errorResponse = new InternalServerErrorResponseModel();
                errorResponse.LogId = Guid.NewGuid().ToString();
                errorResponse.Message = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <response code="200">Returns the list of crypto coins</response>        
        /// <response code="400">If the request is not valid</response>
        /// <response code="401">If the user is not authenticated or authorized</response>
        /// <response code="500">If there is an internal server error</response>        
        [HttpGet("cryptocoin/list")]
        [ProducesResponseType(typeof(List<CryptoCoin>), 200)]
        [ProducesResponseType(typeof(BadRequestResponseModel), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResponseModel), 500)]
        public async Task<IActionResult> CryptoCoinList()
        {
            try
            {
                var query = await _applicationDbContext.CryptoCoin.ToListAsync();
                return StatusCode(200, query);
            }
            catch (Exception exception)
            {
                var errorResponse = new InternalServerErrorResponseModel();
                errorResponse.LogId = Guid.NewGuid().ToString();
                errorResponse.Message = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mark"></param>        
        /// <response code="200">Returns the successful response</response>        
        /// <response code="400">If the request is not valid</response>
        /// <response code="401">If the user is not authenticated or authorized</response>
        /// <response code="500">If there is an internal server error</response>        
        [HttpGet("cryptocoin/markunmark")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(BadRequestResponseModel), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResponseModel), 500)]
        public async Task<IActionResult> CryptoCoinMarkUnmark([FromQuery] string id, [FromQuery] bool mark = true)
        {
            try
            {
                var username = User.Claims.FirstOrDefault(x => x.Type == "Username").Value;
                var user = await _applicationDbContext.User.FirstOrDefaultAsync(x => x.Username == username);
                var cryptoCoin = await _applicationDbContext.CryptoCoin.FirstOrDefaultAsync(x => x.Code == id);
                if (mark)
                {
                    var userCryptoCoin = await _applicationDbContext.UserCryptoCoin.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CryptoCoinId == cryptoCoin.Id);
                    if (userCryptoCoin == null)
                    {
                        userCryptoCoin = new UserCryptoCoin();
                        userCryptoCoin.Id = 0;
                        userCryptoCoin.UserId = user.Id;
                        userCryptoCoin.CryptoCoinId = cryptoCoin.Id;
                        _applicationDbContext.Add(userCryptoCoin);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    var userCryptoCoin = await _applicationDbContext.UserCryptoCoin.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CryptoCoinId == cryptoCoin.Id);
                    if (userCryptoCoin != null)
                    {
                        _applicationDbContext.Entry(userCryptoCoin).State = EntityState.Deleted;
                        await _applicationDbContext.SaveChangesAsync();
                    }
                }
                return StatusCode(200);
            }
            catch (Exception exception)
            {
                var errorResponse = new InternalServerErrorResponseModel();
                errorResponse.LogId = Guid.NewGuid().ToString();
                errorResponse.Message = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <response code="200">Returns the list of user crypto coins</response>        
        /// <response code="400">If the request is not valid</response>
        /// <response code="401">If the user is not authenticated or authorized</response>
        /// <response code="500">If there is an internal server error</response>        
        [HttpGet("cryptocoin/listbyuser")]
        [ProducesResponseType(typeof(List<CryptoCoin>), 200)]
        [ProducesResponseType(typeof(BadRequestResponseModel), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(InternalServerErrorResponseModel), 500)]
        public async Task<IActionResult> CryptoCoinListByUser()
        {
            try
            {
                var username = User.Claims.FirstOrDefault(x => x.Type == "Username").Value;
                var query = await (from userCryptoCoin in _applicationDbContext.UserCryptoCoin
                                   join user in _applicationDbContext.User on userCryptoCoin.UserId equals user.Id
                                   join cryptoCoin in _applicationDbContext.CryptoCoin on userCryptoCoin.CryptoCoinId equals cryptoCoin.Id
                                   where user.Username == username
                                   select cryptoCoin).ToListAsync();
                return StatusCode(200, query);
            }
            catch (Exception exception)
            {
                var errorResponse = new InternalServerErrorResponseModel();
                errorResponse.LogId = Guid.NewGuid().ToString();
                errorResponse.Message = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
                return StatusCode(500, errorResponse);
            }
        }
    }
}