using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SmartEcoA.Models;

namespace SmartEcoA.Controllers
{
    [Route("{language}/api/[controller]")]
    [ApiController]
    public class ReportsController : OnActionExecutingController
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _userManager = userManager;
        }

        // GET: api/Reports
        [HttpGet]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult<IEnumerable<Report>>> GetReport()
        {
            string ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            ApplicationUser applicationUser = await _context.Users.FindAsync(ApplicationUserId);
            var roles = await _userManager.GetRolesAsync(applicationUser);
            if(roles.Contains("Administrator") || roles.Contains("Moderator"))
            {
                return await _context.Report
                    .Include(r => r.ApplicationUser)
                    .ToListAsync();
            }
            else
            {
                return await _context.Report
                    .Where(r => r.ApplicationUserId == ApplicationUserId)
                    .Include(r => r.ApplicationUser)
                    .ToListAsync();
            }
        }

        // GET: api/Reports/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            var report = await _context.Report.FindAsync(id);
            report.ApplicationUser = await _context.Users.FindAsync(report.ApplicationUserId);

            if (report == null)
            {
                return NotFound();
            }

            return report;
        }

        // PUT: api/Reports/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<IActionResult> PutReport(int id, Report report)
        {
            if (id != report.Id)
            {
                return BadRequest();
            }

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
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

        // POST: api/Reports
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult<Report>> PostReport(Report report)
        {
            //_context.Report.Add(report);
            //await _context.SaveChangesAsync();
            report.ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            report.DateTime = DateTime.Now;
            switch (report.NameEN)
            {
                // CarPostDataAutoTestProtocol
                case "Report of measurements of harmful emissions in the exhaust gases of a motor vehicle":
                    report = CreateCarPostDataAutoTestProtocol(report);
                    break;
            }
            _context.Report.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReport", new { id = report.Id }, report);
        }

        private Report CreateCarPostDataAutoTestProtocol(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name}.docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            int carPostDataAutoTestId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            CarPostDataAutoTest carPostDataAutoTest = _context.CarPostDataAutoTest.FirstOrDefault(c => c.Id == carPostDataAutoTestId);
            CarModelAutoTest carModelAutoTest = _context.CarModelAutoTest.FirstOrDefault(c => c.Id == carPostDataAutoTest.CarModelAutoTestId);
            CarPost carPost = _context.CarPost.FirstOrDefault(c => c.Id == carModelAutoTest.CarPostId);

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Date"]}={carPostDataAutoTest.DateTime.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Time"]}={carPostDataAutoTest.DateTime.ToString("HH:mm:ss")};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Date"]}={carPostDataAutoTest.DateTime.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Time"]}={carPostDataAutoTest.DateTime.ToString("HH:mm:ss")};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Date"]}={carPostDataAutoTest.DateTime.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Time"]}={carPostDataAutoTest.DateTime.ToString("HH:mm:ss")};";

            string reportTemplateFileNameFull = Path.Combine(Startup.Configuration["ReportsTeplatesFolder"].ToString(), report.NameRU);
            reportTemplateFileNameFull = Path.ChangeExtension(reportTemplateFileNameFull, "docx");
            System.IO.File.Copy(reportTemplateFileNameFull, reportFileNameFull);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(reportFileNameFull, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                docText = new Regex("CarPostName").Replace(docText, carPost.Name);
                docText = new Regex("Time").Replace(docText, carPostDataAutoTest.DateTime.ToString("HH:mm:ss"));
                docText = new Regex("CarModelName").Replace(docText, carModelAutoTest.Name);
                docText = new Regex("CarNumber").Replace(docText, carPostDataAutoTest.Number);
                docText = new Regex("MIN_TAH").Replace(docText, carPostDataAutoTest.MIN_TAH.HasValue ? carPostDataAutoTest.MIN_TAH.Value.ToString() : string.Empty);
                docText = new Regex("MAX_TAH").Replace(docText, carPostDataAutoTest.MAX_TAH.HasValue ? carPostDataAutoTest.MAX_TAH.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CO").Replace(docText, carPostDataAutoTest.MIN_CO.HasValue ? carPostDataAutoTest.MIN_CO.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CO").Replace(docText, carPostDataAutoTest.MAX_CO.HasValue ? carPostDataAutoTest.MAX_CO.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CH").Replace(docText, carPostDataAutoTest.MIN_CH.HasValue ? carPostDataAutoTest.MIN_CH.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CH").Replace(docText, carPostDataAutoTest.MAX_CH.HasValue ? carPostDataAutoTest.MAX_CH.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CO2").Replace(docText, carPostDataAutoTest.MIN_CO2.HasValue ? carPostDataAutoTest.MIN_CO2.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CO2").Replace(docText, carPostDataAutoTest.MAX_CO2.HasValue ? carPostDataAutoTest.MAX_CO2.Value.ToString() : string.Empty);
                docText = new Regex("MIN_O2").Replace(docText, carPostDataAutoTest.MIN_O2.HasValue ? carPostDataAutoTest.MIN_O2.Value.ToString() : string.Empty);
                docText = new Regex("MAX_O2").Replace(docText, carPostDataAutoTest.MAX_O2.HasValue ? carPostDataAutoTest.MAX_O2.Value.ToString() : string.Empty);
                docText = new Regex("MIN_NO").Replace(docText, carPostDataAutoTest.MIN_NO.HasValue ? carPostDataAutoTest.MIN_NO.Value.ToString() : string.Empty);
                docText = new Regex("MAX_NO").Replace(docText, carPostDataAutoTest.MAX_NO.HasValue ? carPostDataAutoTest.MAX_NO.Value.ToString() : string.Empty);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        // DELETE: api/Reports/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult<Report>> DeleteReport(int id)
        {
            var report = await _context.Report.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            string ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            ApplicationUser applicationUser = await _context.Users.FindAsync(ApplicationUserId);
            var roles = await _userManager.GetRolesAsync(applicationUser);
            if (!roles.Contains("Administrator") && !roles.Contains("Moderator") && report.ApplicationUserId != ApplicationUserId)
            {
                return NotFound();
            }

            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId),
                reportFileNameFull = Path.Combine(userReportFolder, report.FileName);
            if (System.IO.File.Exists(reportFileNameFull))
            {
                System.IO.File.Delete(reportFileNameFull);
            }

            _context.Report.Remove(report);
            await _context.SaveChangesAsync();

            return report;
        }

        // GET: api/Reports/Download/5
        [Route("Download")]
        [HttpGet("Download/{id}")]
        [Authorize(Roles = "Administrator, Moderator, Customer")]
        public async Task<ActionResult> Download(int id)
        {
            var report = await _context.Report.FindAsync(id);

            string ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            ApplicationUser applicationUser = await _context.Users.FindAsync(ApplicationUserId);
            var roles = await _userManager.GetRolesAsync(applicationUser);
            if (!roles.Contains("Administrator") && !roles.Contains("Moderator") && report.ApplicationUserId != ApplicationUserId)
            {
                return NotFound();
            }

            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId),
               reportFileNameFull = Path.Combine(userReportFolder, report.FileName);
            var bytes = await System.IO.File.ReadAllBytesAsync(reportFileNameFull);
            if (Path.GetExtension(reportFileNameFull) == ".docx")
            {
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(reportFileNameFull));
            }
            else
            {
                return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(reportFileNameFull));
            }
        }

        private bool ReportExists(int id)
        {
            return _context.Report.Any(e => e.Id == id);
        }
    }
}
