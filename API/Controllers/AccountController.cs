using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        private readonly UserManager<AppUser> _userManger;
        
        public AccountController(UserManager<AppUser> userManger, DataContext context, ITokenService tokenService, IMapper mapper )
        {
            _userManger = userManger;
            _mapper = mapper;
            _tokenService = tokenService;
                     
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);            
            user.UserName = registerDto.UserName.ToLower();

             var result = await _userManger.CreateAsync(user, registerDto.Password);

             if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManger.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

             return new UserDto
             {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
             }; 
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManger.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName.Trim() == loginDto.Username.Trim());

            if (user == null) return Unauthorized("invalid username");

            var result = await _userManger.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid password");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            }; 
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManger.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
       
    }
}