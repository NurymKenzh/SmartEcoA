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
    public class CarPostDataAutoTestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarPostDataAutoTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarPostDataAutoTests
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarPostDataAutoTest>>> GetCarPostDataAutoTest(
            int? CarPostId,
            DateTime? Date)
        {
            if(CarPostId != null && Date != null)
            {
                return await _context.CarPostDataAutoTest
                    .Where(c => c.DateTime.Year == Date.Value.Year && c.DateTime.Month == Date.Value.Month && c.DateTime.Day == Date.Value.Day)
                    .Include(c => c.CarModelAutoTest)
                    .Where(c => c.CarModelAutoTest.CarPostId == CarPostId)
                    .Include(c => c.CarModelAutoTest.CarPost)
                    .ToListAsync();
            }
            else
            {
                return await _context.CarPostDataAutoTest
                    .Include(c => c.CarModelAutoTest)
                    .Include(c => c.CarModelAutoTest.CarPost)
                    .ToListAsync();
            }
        }

        // GET: api/CarPostDataAutoTests/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataAutoTest>> GetCarPostDataAutoTest(long id)
        {
            var carPostDataAutoTest = await _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.CarPost)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carPostDataAutoTest == null)
            {
                return NotFound();
            }

            return carPostDataAutoTest;
        }

        // PUT: api/CarPostDataAutoTests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarPostDataAutoTest(long id, CarPostDataAutoTest carPostDataAutoTest)
        {
            if (id != carPostDataAutoTest.Id)
            {
                return BadRequest();
            }

            _context.Entry(carPostDataAutoTest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarPostDataAutoTestExists(id))
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

        // POST: api/CarPostDataAutoTests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataAutoTest>> PostCarPostDataAutoTest(CarPostDataAutoTest carPostDataAutoTest)
        {
            _context.CarPostDataAutoTest.Add(carPostDataAutoTest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarPostDataAutoTest", new { id = carPostDataAutoTest.Id }, carPostDataAutoTest);
        }

        // DELETE: api/CarPostDataAutoTests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataAutoTest>> DeleteCarPostDataAutoTest(long id)
        {
            var carPostDataAutoTest = await _context.CarPostDataAutoTest.FindAsync(id);
            if (carPostDataAutoTest == null)
            {
                return NotFound();
            }

            _context.CarPostDataAutoTest.Remove(carPostDataAutoTest);
            await _context.SaveChangesAsync();

            return carPostDataAutoTest;
        }

        private bool CarPostDataAutoTestExists(long id)
        {
            return _context.CarPostDataAutoTest.Any(e => e.Id == id);
        }
    }
}
