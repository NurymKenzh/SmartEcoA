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
    public class PollutionEnvironmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollutionEnvironmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PollutionEnvironments
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<PollutionEnvironment>>> GetPollutionEnvironment()
        {
            return await _context.PollutionEnvironment.ToListAsync();
        }

        // GET: api/PollutionEnvironments/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PollutionEnvironment>> GetPollutionEnvironment(int id)
        {
            var pollutionEnvironment = await _context.PollutionEnvironment.FindAsync(id);

            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            return pollutionEnvironment;
        }

        // PUT: api/PollutionEnvironments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutPollutionEnvironment(int id, PollutionEnvironment pollutionEnvironment)
        {
            if (id != pollutionEnvironment.Id)
            {
                return BadRequest();
            }

            _context.Entry(pollutionEnvironment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollutionEnvironmentExists(id))
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

        // POST: api/PollutionEnvironments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PollutionEnvironment>> PostPollutionEnvironment(PollutionEnvironment pollutionEnvironment)
        {
            _context.PollutionEnvironment.Add(pollutionEnvironment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollutionEnvironment", new { id = pollutionEnvironment.Id }, pollutionEnvironment);
        }

        // DELETE: api/PollutionEnvironments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<PollutionEnvironment>> DeletePollutionEnvironment(int id)
        {
            var pollutionEnvironment = await _context.PollutionEnvironment.FindAsync(id);
            if (pollutionEnvironment == null)
            {
                return NotFound();
            }

            _context.PollutionEnvironment.Remove(pollutionEnvironment);
            await _context.SaveChangesAsync();

            return pollutionEnvironment;
        }

        private bool PollutionEnvironmentExists(int id)
        {
            return _context.PollutionEnvironment.Any(e => e.Id == id);
        }
    }
}
