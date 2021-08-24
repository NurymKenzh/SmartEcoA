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
    public class TypeEcoClassesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeEcoClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TypeEcoClasses
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<TypeEcoClass>>> GetTypeEcoClass()
        {
            return await _context.TypeEcoClass
                .ToListAsync();
        }

        // GET: api/TypeEcoClasses/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<TypeEcoClass>> GetTypeEcoClass(int id)
        {
            var typeEcoClass = await _context.TypeEcoClass
                .FirstOrDefaultAsync(c => c.Id == id);

            if (typeEcoClass == null)
            {
                return NotFound();
            }

            return typeEcoClass;
        }

        // PUT: api/TypeEcoClasses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarModelAutoTest(int id, TypeEcoClass typeEcoClass)
        {
            if (id != typeEcoClass.Id)
            {
                return BadRequest();
            }

            _context.Entry(typeEcoClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeEcoClassExists(id))
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

        // POST: api/TypeEcoClasses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<TypeEcoClass>> PostTypeEcoClass(TypeEcoClass typeEcoClass)
        {
            _context.TypeEcoClass.Add(typeEcoClass);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTypeEcoClass", new { id = typeEcoClass.Id }, typeEcoClass);
        }

        // DELETE: api/TypeEcoClasses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<TypeEcoClass>> DeleteTypeEcoClass(int id)
        {
            var typeEcoClass = await _context.TypeEcoClass.FindAsync(id);
            if (typeEcoClass == null)
            {
                return NotFound();
            }

            _context.TypeEcoClass.Remove(typeEcoClass);
            await _context.SaveChangesAsync();

            return typeEcoClass;
        }

        private bool TypeEcoClassExists(int id)
        {
            return _context.TypeEcoClass.Any(e => e.Id == id);
        }
    }
}