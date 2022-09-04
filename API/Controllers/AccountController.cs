using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entity;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService){
            _tokenService = tokenService;
            _context = context;
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto register){

            if(await UserExists(register.Username)) return BadRequest("Username is taken");


            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = register.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)), // convert to bytes
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); 
            
            return new UserDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            var user = await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if(user == null ) return Unauthorized("Invalid username");

            // calculate if password is correct

            // getting the seeds
            using var hmac = new HMACSHA512(user.PasswordSalt);
            
            // create hash from login user password 
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // compare to the real hash of the user
            for (int i = 0; i < computeHash.Length; i++)
            {
                if(computeHash[i] != user.PasswordHash[i]) 
                    return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username){
           return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}