﻿using System.Threading.Tasks;
using EducationApp.BusinessLogicLayer.Models.Users;
using EducationApp.BusinessLogicLayer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EducationApp.BusinessLogicLayer.Common.Consts.Consts.EmailConsts;
using EducationApp.PresentationLayer.Helpers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using EducationApp.BusinessLogicLayer.Helpers.Mapping;

namespace EducationApp.PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenFactory _tokenFactory;
        private readonly IAccountService _accountService;

        public AccountController(ITokenFactory tokenFactory, IAccountService accountService)
        {
            _tokenFactory = tokenFactory;
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserModelItem userItemModel)
        {
            var result = await _accountService.CreateUserAsync(userItemModel);
            return Ok(result);
        }

        [HttpPost("confirmEmail")]
        public async Task<ActionResult> ConfirmEmail(UserModelItem email)
        {
            var resultConfirmEmail = await _accountService.ConfirmEmailAsync(email.Email);

            return Ok(resultConfirmEmail.Errors);
        }


        [HttpPost("signIn")]
        public async Task<ActionResult> SignIn(string email,string password)
        {
            var errors = await _accountService.SignIn(email, password);
            var user = await _accountService.GetByEmailAsync(email);
            if (errors.Errors.Count > 0)
            {
                return Ok(errors.Errors);
            }
            
            var tokens = _tokenFactory.GenerateTokenModel(user);
            HttpContext.Response.Cookies.Append("RefereshToken", tokens.RefreshToken);
            HttpContext.Response.Cookies.Append("AccessToken", tokens.AccessToken);
            return Ok(errors);
        }
        [Authorize]
        [HttpPost("signOut")]
        public async Task<ActionResult> SignOutAsync()
        {
            await _accountService.SignOutAsync();
            return Ok();
        }
        private async Task<ActionResult> RefreshTokenAsync(string refreshToken)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken);

            if (token.ValidTo >= DateTime.Now)
            {
                var UserId = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                long Id = long.Parse(UserId);
                var user = await _accountService.GetByIdAsync(Id);
                var model = UserMapper.Map(user);
                var encodedJwt = _tokenFactory.GenerateTokenModel(model);
            }
            return Ok();
        }
        private async Task<bool> CheckJwtTokenAsync(string accessToken, string refreshToken)
        {
            var expiresAccess = new JwtSecurityTokenHandler().ReadToken(accessToken).ValidTo;

            if (expiresAccess > DateTime.Now)
            {
                await RefreshTokenAsync(refreshToken);
            }
            return true;
        }





    }
}
