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
    public class CarPostDataSmokeMetersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarPostDataSmokeMetersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarPostDataSmokeMeters
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<CarPostDataSmokeMeter>>> GetCarPostDataSmokeMeter()
        {
            return await _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.CarPost)
                .ToListAsync();
        }

        // GET: api/CarPostDataSmokeMeters/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataSmokeMeter>> GetCarPostDataSmokeMeter(long id)
        {
            var carPostDataSmokeMeter = await _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.CarPost)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carPostDataSmokeMeter == null)
            {
                return NotFound();
            }

            return carPostDataSmokeMeter;
        }

        // PUT: api/CarPostDataSmokeMeters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarPostDataSmokeMeter(long id, CarPostDataSmokeMeter carPostDataSmokeMeter)
        {
            if (id != carPostDataSmokeMeter.Id)
            {
                return BadRequest();
            }

            _context.Entry(carPostDataSmokeMeter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarPostDataSmokeMeterExists(id))
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

        // POST: api/CarPostDataSmokeMeters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataSmokeMeter>> PostCarPostDataSmokeMeter(CarPostDataSmokeMeter carPostDataSmokeMeter)
        {
            _context.CarPostDataSmokeMeter.Add(carPostDataSmokeMeter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarPostDataSmokeMeter", new { id = carPostDataSmokeMeter.Id }, carPostDataSmokeMeter);
        }

        // DELETE: api/CarPostDataSmokeMeters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPostDataSmokeMeter>> DeleteCarPostDataSmokeMeter(long id)
        {
            var carPostDataSmokeMeter = await _context.CarPostDataSmokeMeter.FindAsync(id);
            if (carPostDataSmokeMeter == null)
            {
                return NotFound();
            }

            _context.CarPostDataSmokeMeter.Remove(carPostDataSmokeMeter);
            await _context.SaveChangesAsync();

            return carPostDataSmokeMeter;
        }

        private bool CarPostDataSmokeMeterExists(long id)
        {
            return _context.CarPostDataSmokeMeter.Any(e => e.Id == id);
        }
    }
}
