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
    public class CarPostAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarPostAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarPostAnalytics
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarPostAnalytic>>> GetCarPostAnalytic()
        {
            return await _context.CarPostAnalytic
                .Include(c => c.CarPost)
                .ToListAsync();
        }

        // GET: api/CarPostAnalytics/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostAnalytic>> GetCarPostAnalytic(int id)
        {
            var carPostAnalytic = await _context.CarPostAnalytic
                .Include(c => c.CarPost)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carPostAnalytic == null)
            {
                return NotFound();
            }

            return carPostAnalytic;
        }

        // PUT: api/CarPostAnalytics/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarPostAnalytic(int id, CarPostAnalytic carPostAnalytic)
        {
            if (id != carPostAnalytic.Id)
            {
                return BadRequest();
            }

            _context.Entry(carPostAnalytic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarPostAnalyticExists(id))
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

        // POST: api/CarPostAnalytics
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostAnalytic>> PostCarPostAnalytic(CarPostAnalytic carPostAnalytic)
        {
            _context.CarPostAnalytic.Add(carPostAnalytic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarPostAnalytic", new { id = carPostAnalytic.Id }, carPostAnalytic);
        }

        // DELETE: api/CarPostAnalytics/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostAnalytic>> DeleteCarPostAnalytic(int id)
        {
            var carPostAnalytic = await _context.CarPostAnalytic.FindAsync(id);
            if (carPostAnalytic == null)
            {
                return NotFound();
            }

            _context.CarPostAnalytic.Remove(carPostAnalytic);
            await _context.SaveChangesAsync();

            return carPostAnalytic;
        }

        private bool CarPostAnalyticExists(int id)
        {
            return _context.CarPostAnalytic.Any(e => e.Id == id);
        }
    }
}
