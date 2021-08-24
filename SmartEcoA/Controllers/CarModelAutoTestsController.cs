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
    public class CarModelAutoTestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarModelAutoTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarModelAutoTests
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarModelAutoTest>>> GetCarModelAutoTest(int? carpostid)
        {
            return await _context.CarModelAutoTest
                .Where(c => c.CarPostId == carpostid || carpostid == null)
                .Include(c => c.CarPost)
                .Include(c => c.TypeEcoClass)
                .ToListAsync();
        }

        // GET: api/CarModelAutoTests/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelAutoTest>> GetCarModelAutoTest(int id)
        {
            var carModelAutoTest = await _context.CarModelAutoTest
                .Include(c => c.CarPost)
                .Include(c => c.TypeEcoClass)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carModelAutoTest == null)
            {
                return NotFound();
            }

            return carModelAutoTest;
        }

        // PUT: api/CarModelAutoTests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarModelAutoTest(int id, CarModelAutoTest carModelAutoTest)
        {
            if (id != carModelAutoTest.Id)
            {
                return BadRequest();
            }

            _context.Entry(carModelAutoTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarModelAutoTestExists(id))
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

        // POST: api/CarModelAutoTests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelAutoTest>> PostCarModelAutoTest(CarModelAutoTest carModelAutoTest)
        {
            _context.CarModelAutoTest.Add(carModelAutoTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarModelAutoTest", new { id = carModelAutoTest.Id }, carModelAutoTest);
        }

        // DELETE: api/CarModelAutoTests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarModelAutoTest>> DeleteCarModelAutoTest(int id)
        {
            var carModelAutoTest = await _context.CarModelAutoTest.FindAsync(id);
            if (carModelAutoTest == null)
            {
                return NotFound();
            }

            _context.CarModelAutoTest.Remove(carModelAutoTest);
            await _context.SaveChangesAsync();

            return carModelAutoTest;
        }

        private bool CarModelAutoTestExists(int id)
        {
            return _context.CarModelAutoTest.Any(e => e.Id == id);
        }
    }
}
