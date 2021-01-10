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
    public class PostDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostDatas
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<PostData>>> GetPostData(DateTime? date)
        {
            DateTime dateDay = DateTime.Today;
            if (date != null)
            {
                dateDay = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            }
            return await _context.PostData
                .Where(p => p.DateTime >= dateDay && p.DateTime < dateDay.AddDays(1))
                .ToListAsync();
        }

        // GET: api/PostDatas/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PostData>> GetPostData(long id)
        {
            var postData = await _context.PostData.FindAsync(id);

            if (postData == null)
            {
                return NotFound();
            }

            return postData;
        }

        // PUT: api/PostDatas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutPostData(long id, PostData postData)
        {
            if (id != postData.Id)
            {
                return BadRequest();
            }

            _context.Entry(postData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostDataExists(id))
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

        // POST: api/PostDatas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PostData>> PostPostData(PostData postData)
        {
            _context.PostData.Add(postData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostData", new { id = postData.Id }, postData);
        }

        // DELETE: api/PostDatas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PostData>> DeletePostData(long id)
        {
            var postData = await _context.PostData.FindAsync(id);
            if (postData == null)
            {
                return NotFound();
            }

            _context.PostData.Remove(postData);
            await _context.SaveChangesAsync();

            return postData;
        }

        private bool PostDataExists(long id)
        {
            return _context.PostData.Any(e => e.Id == id);
        }
    }
}
