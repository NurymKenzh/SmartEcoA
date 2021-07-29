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
    public class MeasuredParametersController : OnActionExecutingController
    {
        private readonly ApplicationDbContext _context;

        public MeasuredParametersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MeasuredParameters
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<MeasuredParameter>>> GetMeasuredParameter()
        {
            return await _context.MeasuredParameter.ToListAsync();
        }

        // GET: api/MeasuredParameters/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<MeasuredParameter>> GetMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter.FindAsync(id);

            if (measuredParameter == null)
            {
                return NotFound();
            }

            return measuredParameter;
        }

        // PUT: api/MeasuredParameters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutMeasuredParameter(int id, MeasuredParameter measuredParameter)
        {
            if (id != measuredParameter.Id)
            {
                return BadRequest();
            }

            _context.Entry(measuredParameter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasuredParameterExists(id))
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

        // POST: api/MeasuredParameters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<MeasuredParameter>> PostMeasuredParameter(MeasuredParameter measuredParameter)
        {
            _context.MeasuredParameter.Add(measuredParameter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasuredParameter", new { id = measuredParameter.Id }, measuredParameter);
        }

        // DELETE: api/MeasuredParameters/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<MeasuredParameter>> DeleteMeasuredParameter(int id)
        {
            var measuredParameter = await _context.MeasuredParameter.FindAsync(id);
            if (measuredParameter == null)
            {
                return NotFound();
            }

            _context.MeasuredParameter.Remove(measuredParameter);
            await _context.SaveChangesAsync();

            return measuredParameter;
        }

        private bool MeasuredParameterExists(int id)
        {
            return _context.MeasuredParameter.Any(e => e.Id == id);
        }
    }
}
