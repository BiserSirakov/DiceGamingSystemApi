using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;

using DiceGamingSystemApi.ViewModels.User;
using DiceGamingSystemApi.Models;

namespace DiceGamingSystemApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                VirtualMoney = 0
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Created("DefaultApi", new
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                VirtualMoney = user.VirtualMoney
            });
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/UserInfo
        [Route("UserInfo")]
        public async Task<IHttpActionResult> GetCurrentUserInfo()
        {
            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());

            return Ok(new UserInfoViewModel
            {
                Email = currentUser.Email,
                FullName = currentUser.FullName,
                Username = currentUser.UserName
            });
        }

        // PUT api/Account/ChangeUserInfo
        [HttpPut]
        public async Task<IHttpActionResult> ChangeUserInfo(ChangeUserInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            currentUser.FullName = model.FullName;

            IdentityResult result = await this.UserManager.SetEmailAsync(User.Identity.GetUserId(), model.Email);

            return StatusCode(HttpStatusCode.NoContent);
        }
        
        // PUT api/Account/ChangePassword
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/Account/DeleteUser
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUser(ConfirmUserDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (await this.UserManager.CheckPasswordAsync(currentUser, model.Password))
            {
                IdentityResult result = await this.UserManager.DeleteAsync(currentUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }
            else
            {
                return BadRequest("Wrong password.");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT api/Account/Wallet
        [HttpPut]
        [Route("Wallet")]
        public async Task<IHttpActionResult> Wallet(WalletViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());

            currentUser.VirtualMoney += model.Amount;

            IdentityResult result = await this.UserManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(new { VirtualMoney = currentUser.VirtualMoney });
        }

        // GET api/Account/Wallet
        [HttpGet]
        [Route("Wallet")]
        public async Task<IHttpActionResult> Wallet()
        {
            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());

            return Ok(new { VirtualMoney = currentUser.VirtualMoney });
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
