using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DiceGamingSystemApi.Models;
using DiceGamingSystemApi.ViewModels.Shuffle;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DiceGamingSystemApi.Controllers
{
    public class ShufflesController : ApiController
    {
        private DiceGamingSystemApiDbContext db = new DiceGamingSystemApiDbContext();
        private ApplicationUserManager _userManager;

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

        // POST: api/Shuffles
        public async Task<IHttpActionResult> PostShuffle(ShuffleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Random rnd = new Random();
            int dice1 = rnd.Next(1, 7);
            int dice2 = rnd.Next(1, 7);

            var shuffle = new Shuffle()
            {
                Id = Guid.NewGuid(),
                UserId = User.Identity.GetUserId(),
                Timestamp = DateTime.Now,
                Bet = model.Bet,
                Stake = model.Stake,
                Result = dice1 + dice2
            };

            if (shuffle.Result == model.Bet)
            {
                shuffle.Win = model.Stake * 5;
            }
            else
            {
                shuffle.Win = 0;
            }

            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            currentUser.VirtualMoney += shuffle.Win;

            IdentityResult result = await this.UserManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            db.Shuffles.Add(shuffle);
            await db.SaveChangesAsync();

            return Created("DefaultApi", shuffle);
        }

        // DELETE api/Shuffles/{id}
        public async Task<IHttpActionResult> DeleteShuffle()
        {
            string userId = User.Identity.GetUserId();

            var shuffleToDelete = db.Shuffles.OrderByDescending(x => x.Timestamp).ToList()
                .FirstOrDefault(x => x.UserId == userId && CheckIfDateTimeWithinOneMinute(x.Timestamp));

            if (shuffleToDelete == null)
            {
                return BadRequest("Current user has no shuffles made in the last minute.");
            }

            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());
            currentUser.VirtualMoney -= shuffleToDelete.Win;

            IdentityResult result = await this.UserManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            db.Shuffles.Remove(shuffleToDelete);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET api/Shuffles?skip=0&take=20&orderBy={timestamp|win}&filter={win|lose}
        public async Task<IHttpActionResult> GetShuffles(int skip, int take, string orderBy, string filter = null)
        {
            string userId = User.Identity.GetUserId();

            var shuffles = db.Shuffles.Where(x => x.UserId == userId);

            if (orderBy == "timestamp")
            {
                shuffles = shuffles.OrderBy(x => x.Timestamp);
            }
            else if (orderBy == "win")
            {
                shuffles = shuffles.OrderBy(x => x.Win);
            }
            else
            {
                return BadRequest("You can order the shuffles only by timestamp or win.");
            }

            if (filter == "win")
            {
                shuffles = shuffles.Where(x => x.Win > 0);
            }
            else if (filter == "lose")
            {
                shuffles = shuffles.Where(x => x.Win == 0);
            }

            shuffles = shuffles.Skip(skip).Take(take);

            var result = shuffles.Select(x => new ShufflesListViewModel { Timestamp = x.Timestamp, Stake = x.Stake, Win = x.Win }).ToList();

            return Ok(result);
        }

        // GET api/Shuffles/{id}
        public async Task<IHttpActionResult> GetShuffle(Guid id)
        {
            var shuffle = await db.Shuffles.FirstOrDefaultAsync(x => x.Id == id);

            if (shuffle == null)
            {
                return BadRequest("No shuffle with that id.");
            }

            ShuffleSingleViewModel result = new ShuffleSingleViewModel
            {
                Timestamp = shuffle.Timestamp,
                Stake = shuffle.Stake,
                Win = shuffle.Win,
                Bet = shuffle.Bet,
                Result = shuffle.Result
            };

            return Ok(result);
        }

        private bool CheckIfDateTimeWithinOneMinute(DateTime dt)
        {
            return (DateTime.Now - dt).TotalMinutes < 1;
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
    }
}