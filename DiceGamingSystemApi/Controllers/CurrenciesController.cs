namespace DiceGamingSystemApi.Controllers
{
    using System;
    using System.Collections.Generic;
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
    using Models;
    using ViewModels;
    using DiceGamingSystemApi.ViewModels.User;
    using System.Net;
    using System.Linq;
    using System.Web.Http.Description;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using DiceGamingSystemApi.ViewModels.Currency;

    [Authorize]
    public class CurrenciesController : ApiController
    {
        private DiceGamingSystemApiDbContext db = new DiceGamingSystemApiDbContext();

        // POST api/Currencies
        public async Task<IHttpActionResult> PostCurrency(CurrencyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currencyToAdd = new Currency
            {
                Id = Guid.NewGuid(),
                Value = model.Value,
                UserId = User.Identity.GetUserId()
            };

            db.Currencies.Add(currencyToAdd);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.Created);
        }

        // GET api/Currencies/Balance
        [HttpGet]
        public int Balance()
        {
            string currentUserId = User.Identity.GetUserId();
            return db.Currencies
                .Where(x => x.UserId == currentUserId)
                .Select(x => x.Value)
                .Sum();
        }
    }
}