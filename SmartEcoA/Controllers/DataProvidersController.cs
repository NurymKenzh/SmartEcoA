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
    public class DataProvidersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DataProvidersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DataProviders
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<IEnumerable<DataProvider>>> GetDataProvider()
        {
            return await _context.DataProvider.ToListAsync();
        }

        // GET: api/DataProviders/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<DataProvider>> GetDataProvider(int id)
        {
            var dataProvider = await _context.DataProvider.FindAsync(id);

            if (dataProvider == null)
            {
                return NotFound();
            }

            return dataProvider;
        }

        // PUT: api/DataProviders/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutDataProvider(int id, DataProvider dataProvider)
        {
            if (id != dataProvider.Id)
            {
                return BadRequest();
            }

            _context.Entry(dataProvider).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataProviderExists(id))
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

        // POST: api/DataProviders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<DataProvider>> PostDataProvider(DataProvider dataProvider)
        {
            _context.DataProvider.Add(dataProvider);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDataProvider", new { id = dataProvider.Id }, dataProvider);
        }

        // DELETE: api/DataProviders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<DataProvider>> DeleteDataProvider(int id)
        {
            var dataProvider = await _context.DataProvider.FindAsync(id);
            if (dataProvider == null)
            {
                return NotFound();
            }

            _context.DataProvider.Remove(dataProvider);
            await _context.SaveChangesAsync();

            return dataProvider;
        }

        private bool DataProviderExists(int id)
        {
            return _context.DataProvider.Any(e => e.Id == id);
        }
    }
}
