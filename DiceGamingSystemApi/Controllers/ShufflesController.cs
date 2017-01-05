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

namespace DiceGamingSystemApi.Controllers
{
    

    public class ShufflesController : ApiController
    {
        private DiceGamingSystemApiDbContext db = new DiceGamingSystemApiDbContext();
        
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

            db.Shuffles.Add(shuffle);
            await db.SaveChangesAsync();

            return Created("DefaultApi", shuffle);
        }

        // DELETE api/Shuffles/{id}
        public async Task<IHttpActionResult> DeleteShuffle()
        {
            string userId = User.Identity.GetUserId();
            var shuffles = db.Shuffles;

            var shuffleToDelete = shuffles.ToList()
                .FirstOrDefault(x => x.UserId == userId && CheckIfDateTimeWithinOneMinute(x.Timestamp));

            if (shuffleToDelete == null)
            {
                return BadRequest("Current user has no shuffles made in the last minute.");
            }

            db.Shuffles.Remove(shuffleToDelete);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool CheckIfDateTimeWithinOneMinute(DateTime dt)
        {
            return (DateTime.Now - dt).TotalMinutes < 1;
        }
    }
}