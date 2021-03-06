﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using SmartEcoA.Models;

namespace SmartEcoA.Controllers
{
    [Route("{language}/api/[controller]")]
    [ApiController]
    public class CarPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarPosts
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult<IEnumerable<CarPost>>> GetCarPost()
        {
            return await _context.CarPost.ToListAsync();
        }

        // GET: api/CarPosts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> GetCarPost(int id)
        {
            var carPost = await _context.CarPost.FindAsync(id);

            if (carPost == null)
            {
                return NotFound();
            }

            return carPost;
        }

        // PUT: api/CarPosts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PutCarPost(int id, CarPost carPost)
        {
            if (id != carPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(carPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarPostExists(id))
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

        // POST: api/CarPosts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> PostCarPost(CarPost carPost)
        {
            _context.CarPost.Add(carPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarPost", new { id = carPost.Id }, carPost);
        }

        // DELETE: api/CarPosts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<CarPost>> DeleteCarPost(int id)
        {
            var carPost = await _context.CarPost.FindAsync(id);
            if (carPost == null)
            {
                return NotFound();
            }

            _context.CarPost.Remove(carPost);
            await _context.SaveChangesAsync();

            return carPost;
        }

        private bool CarPostExists(int id)
        {
            return _context.CarPost.Any(e => e.Id == id);
        }

        public class ReportCarPost
        {
            public string CarPostName { get; set; }
            public string EngineFuel { get; set; }
            public int AmountMeasurements { get; set; }
            public int AmountExceedances { get; set; }
            public int? Version { get; set; }
        }

        [HttpPost]
        [Route("Report")]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public ActionResult<IEnumerable<ReportCarPost>> Report(
            [FromBody] JObject content)
        {
            dynamic datas = content;
            DateTime? StartDate = datas.startDate;
            DateTime? EndDate = datas.endDate;
            List<int> CarPostsId = datas.carPostsId.ToObject<List<int>>();

            List<ReportCarPost> reportCarPosts = new List<ReportCarPost>();
            if (StartDate != null && EndDate != null && CarPostsId.Count != 0)
            {
                foreach (var carPostId in CarPostsId)
                {
                    var carPostName = _context.CarPost
                        .Where(c => c.Id == carPostId)
                        .FirstOrDefault()?.Name;

                    //gasoline
                    var carPostDataAutoTest = _context.CarPostDataAutoTest
                        .Include(d => d.CarModelAutoTest)
                        .Where(d => d.DateTime >= StartDate && d.DateTime <= EndDate)
                        .Join(_context.CarModelAutoTest.Where(m => m.CarPostId == carPostId), d => d.CarModelAutoTestId, m => m.Id, (d, m) => d)
                        .ToList();
                    if (carPostDataAutoTest.Count != 0)
                    {
                        int? version = carPostDataAutoTest.FirstOrDefault().Version;
                        if (version == 1 || version == null)
                        {
                            var amountExceedGasoline = carPostDataAutoTest
                                .Where(c => c.MIN_TAH > c.CarModelAutoTest.MIN_TAH || c.MAX_TAH > c.CarModelAutoTest.MAX_TAH || c.MIN_CO > c.CarModelAutoTest.MIN_CO ||
                                    c.MAX_CO > c.CarModelAutoTest.MAX_CO || c.MIN_CH > c.CarModelAutoTest.MIN_CH || c.MAX_CH > c.CarModelAutoTest.MAX_CH ||
                                    c.MIN_L > c.CarModelAutoTest.L_MIN || c.MAX_L > c.CarModelAutoTest.L_MAX || c.K_SVOB > c.CarModelAutoTest.K_SVOB ||
                                    c.K_MAX > c.CarModelAutoTest.K_MAX)
                                .Count();

                            ReportCarPost reportCarPost = new ReportCarPost
                            {
                                CarPostName = carPostName,
                                EngineFuel = "бензин",
                                AmountMeasurements = carPostDataAutoTest.Count(),
                                AmountExceedances = amountExceedGasoline,
                                Version = 1
                            };
                            reportCarPosts.Add(reportCarPost);
                        }
                        else if (version == 2)
                        {
                            var amountExceedGasoline = carPostDataAutoTest
                                .Where(c => c.MIN_TAH > c.CarModelAutoTest.MIN_TAH || c.MAX_TAH > c.CarModelAutoTest.MAX_TAH || c.MIN_CO > c.CarModelAutoTest.MIN_CO ||
                                    c.MAX_CO > c.CarModelAutoTest.MAX_CO || c.MIN_CH > c.CarModelAutoTest.MIN_CH || c.MAX_CH > c.CarModelAutoTest.MAX_CH ||
                                    c.MIN_L > c.CarModelAutoTest.L_MIN || c.MAX_L > c.CarModelAutoTest.L_MAX || c.MIN_CO2 > c.CarModelAutoTest.MIN_CO2 ||
                                    c.MAX_CO2 > c.CarModelAutoTest.MAX_CO2 || c.MIN_O2 > c.CarModelAutoTest.MIN_O2 || c.MAX_O2 > c.CarModelAutoTest.MAX_O2 ||
                                    c.MIN_NOx > c.CarModelAutoTest.MIN_NOx || c.MAX_NOx > c.CarModelAutoTest.MAX_NOx)
                                .Count();

                            ReportCarPost reportCarPost = new ReportCarPost
                            {
                                CarPostName = carPostName,
                                EngineFuel = "бензин",
                                AmountMeasurements = carPostDataAutoTest.Count(),
                                AmountExceedances = amountExceedGasoline,
                                Version = 2
                            };
                            reportCarPosts.Add(reportCarPost);
                        }
                    }
                    //diesel
                    var carPostDataSmokeMeter = _context.CarPostDataSmokeMeter
                        .Include(d => d.CarModelSmokeMeter)
                        .Where(d => d.DateTime >= StartDate && d.DateTime <= EndDate)
                        .Join(_context.CarModelSmokeMeter.Where(m => m.CarPostId == carPostId), d => d.CarModelSmokeMeterId, m => m.Id, (d, m) => d)
                        .ToList();
                    if (carPostDataSmokeMeter.Count != 0)
                    {
                        var amountExceedDiesel = carPostDataSmokeMeter
                            .Where(c => c.DFree > c.CarModelSmokeMeter.DFreeMark || c.DMax > c.CarModelSmokeMeter.DMaxMark)
                            .Count();

                        ReportCarPost reportCarPost = new ReportCarPost
                        {
                            CarPostName = carPostName,
                            EngineFuel = "дизель",
                            AmountMeasurements = carPostDataSmokeMeter.Count(),
                            AmountExceedances = amountExceedDiesel
                        };
                        reportCarPosts.Add(reportCarPost);
                    }
                }
            }
            return reportCarPosts;
        }
    }
}
