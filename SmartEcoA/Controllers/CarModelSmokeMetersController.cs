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
    public class CarModelSmokeMetersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarModelSmokeMetersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarModelSmokeMeters
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarModelSmokeMeter>>> GetCarModelSmokeMeter(int? carpostid)
        {
            return await _context.CarModelSmokeMeter
                .Where(c => c.CarPostId == carpostid || carpostid == null)
                .Include(c => c.CarPost)
                .Include(c => c.TypeEcoClass)
                .ToListAsync();
        }

        // GET: api/CarModelSmokeMeters/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelSmokeMeter>> GetCarModelSmokeMeter(int id)
        {
            var carModelSmokeMeter = await _context.CarModelSmokeMeter
                .Include(c => c.CarPost)
                .Include(c => c.TypeEcoClass)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carModelSmokeMeter == null)
            {
                return NotFound();
            }

            return carModelSmokeMeter;
        }

        // PUT: api/CarModelSmokeMeters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarModelSmokeMeter(int id, CarModelSmokeMeter carModelSmokeMeter)
        {
            if (id != carModelSmokeMeter.Id)
            {
                return BadRequest();
            }

            _context.Entry(carModelSmokeMeter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarModelSmokeMeterExists(id))
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

        // POST: api/CarModelSmokeMeters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelSmokeMeter>> PostCarModelSmokeMeter(CarModelSmokeMeter carModelSmokeMeter)
        {
            _context.CarModelSmokeMeter.Add(carModelSmokeMeter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarModelSmokeMeter", new { id = carModelSmokeMeter.Id }, carModelSmokeMeter);
        }

        // DELETE: api/CarModelSmokeMeters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelSmokeMeter>> DeleteCarModelSmokeMeter(int id)
        {
            var carModelSmokeMeter = await _context.CarModelSmokeMeter.FindAsync(id);
            if (carModelSmokeMeter == null)
            {
                return NotFound();
            }

            _context.CarModelSmokeMeter.Remove(carModelSmokeMeter);
            await _context.SaveChangesAsync();

            return carModelSmokeMeter;
        }

        private bool CarModelSmokeMeterExists(int id)
        {
            return _context.CarModelSmokeMeter.Any(e => e.Id == id);
        }
    }
}
