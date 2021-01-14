using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoA.Models;

namespace SmartEcoA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostDataDividedsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostDataDividedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostDataDivideds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDataDivided>>> GetPostDataDivided()
        {
            return await _context.PostDataDivided.ToListAsync();
        }

        // GET: api/PostDataDivideds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDataDivided>> GetPostDataDivided(long id)
        {
            var postDataDivided = await _context.PostDataDivided.FindAsync(id);

            if (postDataDivided == null)
            {
                return NotFound();
            }

            return postDataDivided;
        }

        // PUT: api/PostDataDivideds/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostDataDivided(long id, PostDataDivided postDataDivided)
        {
            if (id != postDataDivided.Id)
            {
                return BadRequest();
            }

            _context.Entry(postDataDivided).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostDataDividedExists(id))
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

        // POST: api/PostDataDivideds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PostDataDivided>> PostPostDataDivided(PostDataDivided postDataDivided)
        {
            _context.PostDataDivided.Add(postDataDivided);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostDataDivided", new { id = postDataDivided.Id }, postDataDivided);
        }

        // DELETE: api/PostDataDivideds/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostDataDivided>> DeletePostDataDivided(long id)
        {
            var postDataDivided = await _context.PostDataDivided.FindAsync(id);
            if (postDataDivided == null)
            {
                return NotFound();
            }

            _context.PostDataDivided.Remove(postDataDivided);
            await _context.SaveChangesAsync();

            return postDataDivided;
        }

        private bool PostDataDividedExists(long id)
        {
            return _context.PostDataDivided.Any(e => e.Id == id);
        }
    }
}
