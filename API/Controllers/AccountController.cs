using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountsController : BaseApiController
    {
        private DataContext _dataContext;
        private ITokenService _tokenService;

        public AccountsController(DataContext dataContext, ITokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await DoesUserExist(registerDTO.Username))
            {
                return BadRequest("Username is taken");
            }

            using (var hmac = new HMACSHA512())
            {
                var user = new AppUser
                {
                    UserName = registerDTO.Username,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                    PasswordSalt = hmac.Key
                };

                _dataContext.Add(user);
                await _dataContext.SaveChangesAsync();

                return new UserDTO
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Log(LogInDTO logInDTO)
        {
            var user = await _dataContext.AppUsers.FirstOrDefaultAsync(x => x.UserName == logInDTO.Username);

            if(user == null)
            {
                return Unauthorized("User doesn't exist");
            }

            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logInDTO.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
                }

                return new UserDTO
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }
        }

        private async Task<bool> DoesUserExist(string userName)
        {
            var DoesUserExists = await _dataContext.AppUsers.AnyAsync(x => x.UserName == userName);

            return DoesUserExists;
        }
    }
}