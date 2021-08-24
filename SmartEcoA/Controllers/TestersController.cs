using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoA.Models;

namespace SmartEcoA.Controllers
{
    [Route("{language}/api/[controller]")]
    [ApiController]
    public class TestersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Testers
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<Tester>>> GetTester()
        {
            return await _context.Tester
                .ToListAsync();
        }

        // GET: api/Testers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Tester>> GetTester(int id)
        {
            var tester = await _context.Tester
                .FirstOrDefaultAsync(c => c.Id == id);

            if (tester == null)
            {
                return NotFound();
            }

            return tester;
        }

        // PUT: api/Testers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutTester(int id, Tester tester)
        {
            if (id != tester.Id)
            {
                return BadRequest();
            }

            _context.Entry(tester).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TesterExists(id))
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

        // POST: api/Testers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Tester>> PostTester(Tester tester)
        {
            _context.Tester.Add(tester);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTester", new { id = tester.Id }, tester);
        }

        // DELETE: api/Testers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Tester>> DeleteTester(int id)
        {
            var tester = await _context.Tester.FindAsync(id);
            if (tester == null)
            {
                return NotFound();
            }

            _context.Tester.Remove(tester);
            await _context.SaveChangesAsync();

            return tester;
        }

        private bool TesterExists(int id)
        {
            return _context.Tester.Any(e => e.Id == id);
        }
    }
}