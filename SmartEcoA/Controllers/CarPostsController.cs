using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoA.Models;

namespace SmartEcoA.Controllers
{
    [Route("{language}/api/[controller]")]
    [ApiController]
    public class CarPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarPosts
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarPost>>> GetCarPost()
        {
            return await _context.CarPost.ToListAsync();
        }

        // GET: api/CarPosts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> GetCarPost(int id)
        {
            var carPost = await _context.CarPost.FindAsync(id);

            if (carPost == null)
            {
                return NotFound();
            }

            return carPost;
        }

        // PUT: api/CarPosts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarPost(int id, CarPost carPost)
        {
            if (id != carPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(carPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CarPosts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> PostCarPost(CarPost carPost)
        {
            _context.CarPost.Add(carPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarPost", new { id = carPost.Id }, carPost);
        }

        // DELETE: api/CarPosts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> DeleteCarPost(int id)
        {
            var carPost = await _context.CarPost.FindAsync(id);
            if (carPost == null)
            {
                return NotFound();
            }

            _context.CarPost.Remove(carPost);
            await _context.SaveChangesAsync();

            return carPost;
        }

        private bool CarPostExists(int id)
        {
            return _context.CarPost.Any(e => e.Id == id);
        }
    }
}
