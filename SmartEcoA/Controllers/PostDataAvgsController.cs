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
    public class PostDataAvgsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostDataAvgsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostDataAvgs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDataAvg>>> GetPostDataAvg()
        {
            return await _context.PostDataAvg.ToListAsync();
        }

        // GET: api/PostDataAvgs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDataAvg>> GetPostDataAvg(long id)
        {
            var postDataAvg = await _context.PostDataAvg.FindAsync(id);

            if (postDataAvg == null)
            {
                return NotFound();
            }

            return postDataAvg;
        }

        // PUT: api/PostDataAvgs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostDataAvg(long id, PostDataAvg postDataAvg)
        {
            if (id != postDataAvg.Id)
            {
                return BadRequest();
            }

            _context.Entry(postDataAvg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostDataAvgExists(id))
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

        // POST: api/PostDataAvgs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PostDataAvg>> PostPostDataAvg(PostDataAvg postDataAvg)
        {
            _context.PostDataAvg.Add(postDataAvg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostDataAvg", new { id = postDataAvg.Id }, postDataAvg);
        }

        // DELETE: api/PostDataAvgs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostDataAvg>> DeletePostDataAvg(long id)
        {
            var postDataAvg = await _context.PostDataAvg.FindAsync(id);
            if (postDataAvg == null)
            {
                return NotFound();
            }

            _context.PostDataAvg.Remove(postDataAvg);
            await _context.SaveChangesAsync();

            return postDataAvg;
        }

        private bool PostDataAvgExists(long id)
        {
            return _context.PostDataAvg.Any(e => e.Id == id);
        }
    }
}
