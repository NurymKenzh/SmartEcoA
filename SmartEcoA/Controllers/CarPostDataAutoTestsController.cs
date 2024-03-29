﻿using System;
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
            var carPostDataAutoTest = _context.CarPostDataAutoTest
                    .Include(c => c.CarModelAutoTest)
                    .Include(c => c.CarModelAutoTest.CarPost)
                    .Include(c => c.Tester)
                    .Where(c => true);

            if (Date != null)
            {
                carPostDataAutoTest = carPostDataAutoTest.Where(c => c.DateTime.Value.Year == Date.Value.Year && c.DateTime.Value.Month == Date.Value.Month && c.DateTime.Value.Day == Date.Value.Day);
            }
            if (CarPostId != null)
            {
                carPostDataAutoTest = carPostDataAutoTest.Where(c => c.CarModelAutoTest.CarPostId == CarPostId);
            }

            return await carPostDataAutoTest.ToListAsync();
        }

        // GET: api/CarPostDataAutoTests/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataAutoTest>> GetCarPostDataAutoTest(long id)
        {
            var carPostDataAutoTest = await _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.CarPost)
                .Include(c => c.Tester)
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
