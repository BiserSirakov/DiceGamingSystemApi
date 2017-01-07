using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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
            var currentUser = await this.UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (currentUser.VirtualMoney < model.Stake)
            {
                return BadRequest("You do not have enough virtual money.");
            }

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
                shuffle.Win = this.CalculateWin(shuffle.Result, model.Stake);
            }
            else
            {
                shuffle.Win = -model.Stake;
            }
            
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

        // DELETE api/Shuffles
        public async Task<IHttpActionResult> DeleteShuffle()
        {
            string userId = User.Identity.GetUserId();

            var shuffleToDelete = db.Shuffles.OrderByDescending(x => x.Timestamp).ToList()
                .FirstOrDefault(x => x.UserId == userId && CheckIfDateTimeWithinOneMinute(x.Timestamp));

            if (shuffleToDelete == null)
            {
                return BadRequest("Current user has no shuffles made in the last minute.");
            }

            var currentUser = await this.UserManager.FindByIdAsync(userId);
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
        public IHttpActionResult GetShuffles(int skip, int take, string orderBy, string filter = null)
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
                shuffles = shuffles.Where(x => x.Win < 0);
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

        private int CalculateWin(int result, int stake)
        {
            int winMultiplier = 1;

            // Number of cominations for different results
            //  2 -> 1
            //  3 -> 2
            //  4 -> 4
            //  5 -> 4
            //  6 -> 6
            //  7 -> 6
            //  8 -> 6
            //  9 -> 4
            // 10 -> 4
            // 11 -> 2
            // 12 -> 1

            switch (result)
            {
                case 2:
                case 12:
                    winMultiplier = 5;
                    break;
                case 3:
                case 11:
                    winMultiplier = 4;
                    break;
                case 4:
                case 5:
                case 9:
                case 10:
                    winMultiplier = 3;
                    break;
                case 6:
                case 7:
                case 8:
                    winMultiplier = 2;
                    break;
            }

            return stake * winMultiplier;
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