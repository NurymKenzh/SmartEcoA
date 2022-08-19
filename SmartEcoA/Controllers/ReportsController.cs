using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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
        public async Task<ActionResult<IEnumerable<Report>>> GetReport(DateTime? Date)
        {
            string ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            ApplicationUser applicationUser = await _context.Users.FindAsync(ApplicationUserId);
            var roles = await _userManager.GetRolesAsync(applicationUser);
            if(roles.Contains("Administrator") || roles.Contains("Moderator"))
            {
                if (Date != null)
                {
                    return await _context.Report
                        .Include(r => r.ApplicationUser)
                        .Where(r => r.DateTime.Value.Year == Date.Value.Year && r.DateTime.Value.Month == Date.Value.Month && r.DateTime.Value.Day == Date.Value.Day)
                        .ToListAsync();
                }

                return await _context.Report
                    .Include(r => r.ApplicationUser)
                    .ToListAsync();
            }
            else
            {
                if (Date != null)
                {
                    return await _context.Report
                        .Where(r => r.ApplicationUserId == ApplicationUserId)
                        .Include(r => r.ApplicationUser)
                        .Where(r => r.DateTime.Value.Year == Date.Value.Year && r.DateTime.Value.Month == Date.Value.Month && r.DateTime.Value.Day == Date.Value.Day)
                        .ToListAsync();
                }

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
            try
            {
            switch (report.NameEN)
            {
                // CarPostDataAutoTestProtocol
                case "Report of measurements of harmful emissions in the exhaust gases of a motor vehicle":
                    report = CreateCarPostDataAutoTestProtocol(report);
                    break;
                // CarPostDataSmokeMeterProtocol
                case "Report of measurements of harmful emissions in the exhaust gases of a motor vehicle (Diesel)":
                    report = CreateCarPostDataSmokeMeterProtocol(report);
                    break;
                // CarPostDataSmokeMeterLog
                case "Vehicle Emission Test Results Log (Diesel)":
                    report = CreateCarPostDataSmokeMeterLog(report);
                    break;
                // CarPostDataAutoTestLog
                case "Vehicle Emission Test Results Log (Gasoline)":
                    report = CreateCarPostDataAutoTestLog(report);
                    break;
                // CarPostsProtocol
                case "Report car posts for the period":
                    report = CreateCarPostsProtocol(report);
                    break;
                // CarsExcessProtocol
                case "Report on repeated exceedances cars at posts":
                    report = CreateCarsExcessProtocol(report);
                    break;
                // CarPostDataAutoTestProtocolPeriod
                case "Report of measurements of harmful emissions in the exhaust gases of a motor vehicle for the period":
                    report = CreateCarPostDataAutoTestProtocolPeriod(report);
                    break;
                // CreateCarPostDataSmokeMeterProtocolPeriod
                case "Report of measurements of harmful emissions in the exhaust gases of a motor vehicle (Diesel) for the period":
                    report = CreateCarPostDataSmokeMeterProtocolPeriod(report);
                    break;
                }
                if (report.PDF && !report.FileName.Contains(".zip"))
                {
                    var wordName = report.FileName;
                    report.FileName = report.FileName.Replace("(MS Word).docx", "(PDF).pdf");
                    ConvertDocToPdf(Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId), report.FileName, wordName);
                }
            }
            catch(Exception ex)
            {

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
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            int carPostDataAutoTestId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            CarPostDataAutoTest carPostDataAutoTest = _context.CarPostDataAutoTest
                .Include(c => c.Tester)
                .FirstOrDefault(c => c.Id == carPostDataAutoTestId);
            CarModelAutoTest carModelAutoTest = _context.CarModelAutoTest
                .Include(c => c.TypeEcoClass)
                .FirstOrDefault(c => c.Id == carPostDataAutoTest.CarModelAutoTestId);
            CarPost carPost = _context.CarPost.FirstOrDefault(c => c.Id == carModelAutoTest.CarPostId);
            var carCheckNumber = _context.CarPostDataAutoTest
                .Where(c => c.DateTime <= carPostDataAutoTest.DateTime && c.Number == carPostDataAutoTest.Number)
                .Count()
                .ToString();
            
            var typeEcoClasses = _context.TypeEcoClass.ToList();

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Date"]}={carPostDataAutoTest.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Time"]}={carPostDataAutoTest.DateTime.Value.ToString("HH:mm:ss")};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Date"]}={carPostDataAutoTest.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Time"]}={carPostDataAutoTest.DateTime.Value.ToString("HH:mm:ss")};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Date"]}={carPostDataAutoTest.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarNumber"]}={carPostDataAutoTest.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarModel"]}={carModelAutoTest.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Time"]}={carPostDataAutoTest.DateTime.Value.ToString("HH:mm:ss")};";

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

                docText = new Regex("TestNumb").Replace(docText, carPostDataAutoTest.TestNumber.HasValue ? carPostDataAutoTest.TestNumber.Value.ToString() : string.Empty);
                docText = new Regex("Day").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("dd"));
                docText = new Regex("Month").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("MM"));
                docText = new Regex("CarPostName").Replace(docText, carPost.Name);
                docText = new Regex("Time").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("HH:mm:ss"));
                docText = new Regex("GasSerialNumber").Replace(docText, carPostDataAutoTest.GasSerialNumber.HasValue ? carPostDataAutoTest.GasSerialNumber.Value.ToString() : string.Empty);
                docText = new Regex("GasCheckDate").Replace(docText, carPostDataAutoTest.GasCheckDate.HasValue ? carPostDataAutoTest.GasCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                docText = new Regex("MeteoSerialNumber").Replace(docText, carPostDataAutoTest.MeteoSerialNumber.HasValue ? carPostDataAutoTest.MeteoSerialNumber.Value.ToString() : string.Empty);
                docText = new Regex("MeteoCheckDate").Replace(docText, carPostDataAutoTest.MeteoCheckDate.HasValue ? carPostDataAutoTest.MeteoCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                docText = new Regex("Temperature").Replace(docText, carPostDataAutoTest.Temperature.HasValue ? carPostDataAutoTest.Temperature.Value.ToString() : string.Empty);
                docText = new Regex("Pressure").Replace(docText, carPostDataAutoTest.Pressure.HasValue ? carPostDataAutoTest.Pressure.Value.ToString() : string.Empty);
                docText = new Regex("CarModelName").Replace(docText, carModelAutoTest.Name);
                docText = new Regex("CarNumber").Replace(docText, carPostDataAutoTest.Number);
                docText = new Regex("Eco").Replace(docText, typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataAutoTest.DOPOL2.ToLower()))?.Name.Split(' ')[0]);
                docText = new Regex("Cat").Replace(docText, carModelAutoTest.Category);
                docText = new Regex("Check").Replace(docText, carCheckNumber);
                docText = new Regex("CarYear").Replace(docText, carPostDataAutoTest.DOPOL1);
                docText = new Regex("MIN_TAH").Replace(docText, carPostDataAutoTest.MIN_TAH.HasValue ? carPostDataAutoTest.MIN_TAH.Value.ToString() : string.Empty);
                docText = new Regex("MAX_TAH").Replace(docText, carPostDataAutoTest.MAX_TAH.HasValue ? carPostDataAutoTest.MAX_TAH.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CO_").Replace(docText, carPostDataAutoTest.MIN_CO.HasValue ? carPostDataAutoTest.MIN_CO.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CO_").Replace(docText, carPostDataAutoTest.MAX_CO.HasValue ? carPostDataAutoTest.MAX_CO.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CH_").Replace(docText, carPostDataAutoTest.MIN_CH.HasValue ? carPostDataAutoTest.MIN_CH.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CH_").Replace(docText, carPostDataAutoTest.MAX_CH.HasValue ? carPostDataAutoTest.MAX_CH.Value.ToString() : string.Empty);
                docText = new Regex("MIN_CO2").Replace(docText, carPostDataAutoTest.MIN_CO2.HasValue ? carPostDataAutoTest.MIN_CO2.Value.ToString() : string.Empty);
                docText = new Regex("MAX_CO2").Replace(docText, carPostDataAutoTest.MAX_CO2.HasValue ? carPostDataAutoTest.MAX_CO2.Value.ToString() : string.Empty);
                docText = new Regex("MIN_O2_").Replace(docText, carPostDataAutoTest.MIN_O2.HasValue ? carPostDataAutoTest.MIN_O2.Value.ToString() : string.Empty);
                docText = new Regex("MAX_O2_").Replace(docText, carPostDataAutoTest.MAX_O2.HasValue ? carPostDataAutoTest.MAX_O2.Value.ToString() : string.Empty);
                docText = new Regex("MIN_NO_").Replace(docText, carPostDataAutoTest.MIN_NO.HasValue ? carPostDataAutoTest.MIN_NO.Value.ToString() : string.Empty);
                docText = new Regex("MAX_NO_").Replace(docText, carPostDataAutoTest.MAX_NO.HasValue ? carPostDataAutoTest.MAX_NO.Value.ToString() : string.Empty);
                docText = new Regex("Tester").Replace(docText, carPostDataAutoTest.Tester?.Name != null ? carPostDataAutoTest.Tester.Name.Split(' ')[0] : string.Empty);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        private Report CreateCarPostDataSmokeMeterProtocol(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            int carPostDataSmokeMeterId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            CarPostDataSmokeMeter carPostDataSmokeMeter = _context.CarPostDataSmokeMeter
                .Include(c => c.Tester)
                .FirstOrDefault(c => c.Id == carPostDataSmokeMeterId);
            CarModelSmokeMeter carModelSmokeMeter = _context.CarModelSmokeMeter
                .Include(c => c.TypeEcoClass)
                .FirstOrDefault(c => c.Id == carPostDataSmokeMeter.CarModelSmokeMeterId);
            CarPost carPost = _context.CarPost.FirstOrDefault(c => c.Id == carModelSmokeMeter.CarPostId);
            var carCheckNumber = _context.CarPostDataSmokeMeter
                .Where(c => c.DateTime <= carPostDataSmokeMeter.DateTime && c.Number == carPostDataSmokeMeter.Number)
                .Count()
                .ToString();
            
            var typeEcoClasses = _context.TypeEcoClass.ToList();

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Date"]}={carPostDataSmokeMeter.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarNumber"]}={carPostDataSmokeMeter.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarModel"]}={carModelSmokeMeter.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["Time"]}={carPostDataSmokeMeter.DateTime.Value.ToString("HH:mm:ss")};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Date"]}={carPostDataSmokeMeter.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarNumber"]}={carPostDataSmokeMeter.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarModel"]}={carModelSmokeMeter.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["Time"]}={carPostDataSmokeMeter.DateTime.Value.ToString("HH:mm:ss")};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Date"]}={carPostDataSmokeMeter.DateTime.Value.ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPost"]}={carPost.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarNumber"]}={carPostDataSmokeMeter.Number};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarModel"]}={carModelSmokeMeter.Name};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["Time"]}={carPostDataSmokeMeter.DateTime.Value.ToString("HH:mm:ss")};";

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

                docText = new Regex("TestNumb").Replace(docText, carPostDataSmokeMeter.TestNumber.HasValue ? carPostDataSmokeMeter.TestNumber.Value.ToString() : string.Empty);
                docText = new Regex("Day").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("dd"));
                docText = new Regex("Month").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("MM"));
                docText = new Regex("CarPostName").Replace(docText, carPost.Name);
                docText = new Regex("Time").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("HH:mm:ss"));
                docText = new Regex("GasSerialNumber").Replace(docText, carPostDataSmokeMeter.GasSerialNumber.HasValue ? carPostDataSmokeMeter.GasSerialNumber.Value.ToString() : string.Empty);
                docText = new Regex("GasCheckDate").Replace(docText, carPostDataSmokeMeter.GasCheckDate.HasValue ? carPostDataSmokeMeter.GasCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                docText = new Regex("MeteoSerialNumber").Replace(docText, carPostDataSmokeMeter.MeteoSerialNumber.HasValue ? carPostDataSmokeMeter.MeteoSerialNumber.Value.ToString() : string.Empty);
                docText = new Regex("MeteoCheckDate").Replace(docText, carPostDataSmokeMeter.MeteoCheckDate.HasValue ? carPostDataSmokeMeter.MeteoCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                docText = new Regex("Temperature").Replace(docText, carPostDataSmokeMeter.Temperature.HasValue ? carPostDataSmokeMeter.Temperature.Value.ToString() : string.Empty);
                docText = new Regex("Pressure").Replace(docText, carPostDataSmokeMeter.Pressure.HasValue ? carPostDataSmokeMeter.Pressure.Value.ToString() : string.Empty);
                docText = new Regex("CarModelName").Replace(docText, carModelSmokeMeter.Name);
                docText = new Regex("CarNumber").Replace(docText, carPostDataSmokeMeter.Number);
                docText = new Regex("Check").Replace(docText, carCheckNumber);
                docText = new Regex("Eco").Replace(docText, typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataSmokeMeter.DOPOL2.ToLower()))?.Name.Split(' ')[0]);
                docText = new Regex("Cat").Replace(docText, carModelSmokeMeter.Category);
                docText = new Regex("CarYear").Replace(docText, carPostDataSmokeMeter.DOPOL1);
                docText = new Regex(@"\b(DFree)\b").Replace(docText, carPostDataSmokeMeter.K_SVOB.HasValue ? carPostDataSmokeMeter.K_SVOB.Value.ToString() : string.Empty);
                docText = new Regex(@"\b(NDFree)\b").Replace(docText, carPostDataSmokeMeter.K_MAX.HasValue ? carPostDataSmokeMeter.K_MAX.Value.ToString() : string.Empty);
                docText = new Regex("Tester").Replace(docText, carPostDataSmokeMeter.Tester?.Name != null ? carPostDataSmokeMeter.Tester.Name.Split(' ')[0] : string.Empty);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        private Report CreateCarPostDataSmokeMeterLog(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            int carPostId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            CarPost carPost = _context.CarPost.FirstOrDefault(c => c.Id == carPostId);
            List<CarPostDataSmokeMeter> carPostDataSmokeMeters = _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.TypeEcoClass)
                .Include(c => c.Tester)
                .Where(c => c.CarModelSmokeMeter.CarPostId == carPost.Id && report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .OrderBy(c => c.DateTime)
                .ToList();
            
            var typeEcoClasses = _context.TypeEcoClass.ToList();

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPost"]}={carPost.Name};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPost"]}={carPost.Name};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPost"]}={carPost.Name};";

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
                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                Body body = wordDoc.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                for (int i = 0; i < carPostDataSmokeMeters.Count; i++)
                {
                    table.Append(
                        new TableRow(
                            new TableCell(new Paragraph(new Run(new Text($"{i + 1}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].DateTime.Value.ToString("dd.MM.yyyy")}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].DateTime.Value.ToString("HH:mm:ss")}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].CarModelSmokeMeter.Name}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataSmokeMeters[i].DOPOL2.ToLower()))?.Name.Split(' ')[0]}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].CarModelSmokeMeter.Category}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].DOPOL1}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].Number}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].K_SVOB}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataSmokeMeters[i].K_MAX}")))),
                            new TableCell(new Paragraph(new Run(new Text(string.Empty))))));
                }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        private Report CreateCarPostDataAutoTestLog(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            int carPostId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            CarPost carPost = _context.CarPost.FirstOrDefault(c => c.Id == carPostId);
            List<CarPostDataAutoTest> carPostDataAutoTests = _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.TypeEcoClass)
                .Include(c => c.Tester)
                .Where(c => c.CarModelAutoTest.CarPostId == carPost.Id && report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .OrderBy(c => c.DateTime)
                .ToList();

            var typeEcoClasses = _context.TypeEcoClass.ToList();

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPost"]}={carPost.Name};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPost"]}={carPost.Name};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPost"]}={carPost.Name};";

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
                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                Body body = wordDoc.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                for (int i = 0; i < carPostDataAutoTests.Count; i++)
                {
                    table.Append(
                        new TableRow(
                            new TableCell(new Paragraph(new Run(new Text($"{i + 1}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].DateTime.Value.ToString("dd.MM.yyyy")}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].DateTime.Value.ToString("HH:mm:ss")}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].CarModelAutoTest.Name}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataAutoTests[i].DOPOL2.ToLower()))?.Name.Split(' ')[0]}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].CarModelAutoTest.Category}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].DOPOL1}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].Number}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataAutoTests[i].DOPOL2.ToLower()))?.MIN_CO}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].MAX_CO}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].MAX_CH}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].MAX_CO2}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].MAX_O2}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{carPostDataAutoTests[i].MAX_NO}")))),
                            new TableCell(new Paragraph(new Run(new Text($"{(carPostDataAutoTests[i].Tester?.Name != null ? carPostDataAutoTests[i].Tester.Name.Split(' ')[0] : string.Empty)}")))),
                            new TableCell(new Paragraph(new Run(new Text(string.Empty))))));
                }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        private Report CreateCarPostsProtocol(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} c {report.CarPostStartDate.Value.ToString("dd-MM-yyyy")} по {report.CarPostEndDate.Value.ToString("dd-MM-yyyy")} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            var carPostsDataAutoTest = _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.CarPost)
                //.Where(c => c.DateTime.Value.Year == report.CarPostStartDate.Value.Year && c.DateTime.Value.Month == report.CarPostStartDate.Value.Month && c.DateTime.Value.Day == report.CarPostStartDate.Value.Day)
                .Where(c => report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .ToList();
            var carPostsDataSmokeMeter = _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.CarPost)
                //.Where(c => c.DateTime.Value.Year == report.CarPostStartDate.Value.Year && c.DateTime.Value.Month == report.CarPostStartDate.Value.Month && c.DateTime.Value.Day == report.CarPostStartDate.Value.Day)
                .Where(c => report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .ToList();

            var carPosts = _context.CarPost
                .ToList();

            //report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostStartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};";
            //report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostStartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};";
            //report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostStartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};";

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";

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

                //docText = new Regex("MeasuredDate").Replace(docText, report.CarPostStartDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + " г.");
                docText = new Regex("StartDate").Replace(docText, report.CarPostStartDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + " г.");
                docText = new Regex("EndDate").Replace(docText, report.CarPostEndDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + " г.");

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                Body body = wordDoc.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                List<TableRow> rows = table.Elements<TableRow>().ToList();
                int fontSize = 20;

                var typeEcoClasses = _context.TypeEcoClass.ToList();

                foreach (var carPost in carPosts)
                {
                    var carPostDataAutoTest = carPostsDataAutoTest
                        .Where(c => c.CarModelAutoTest.CarPost.Id == carPost.Id);

                    var carPostDataSmokeMeter = carPostsDataSmokeMeter
                        .Where(c => c.CarModelSmokeMeter.CarPost.Id == carPost.Id);

                    var amountExceedGasoline = carPostDataAutoTest
                        .Where(c => c.MIN_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CO 
                        || c.MAX_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CO 
                        || c.MIN_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CH 
                        || c.MAX_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CH)
                        .Count();

                    var amountExceedDiesel = carPostDataSmokeMeter
                        .Where(c => c.K_SVOB > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_SVOB 
                        || c.K_MAX > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_MAX)
                        .Count();

                    var amountExceedCO = carPostDataAutoTest
                        .Where(c => c.MIN_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CO 
                        || c.MAX_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CO)
                        .Count();

                    var amountExceedKSVOB = carPostDataSmokeMeter
                        .Where(c => c.K_SVOB > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_SVOB)
                        .Count();

                    var postNames = carPost.Name.Split(' ');
                    var name = postNames.Length > 1 ? postNames[1] : postNames.Length > 0 ? postNames[0] : "";
                    rows[0].Append(SetTableCell($"{name.Replace("\"", "")}", fontSize));
                    rows[1].Append(SetTableCell($"{carPostDataAutoTest.Count() + carPostDataSmokeMeter.Count()}", fontSize));
                    rows[2].Append(SetTableCell($"{carPostDataAutoTest.Count()}", fontSize));
                    rows[3].Append(SetTableCell($"{carPostDataSmokeMeter.Count()}", fontSize));
                    rows[4].Append(SetTableCell($"{amountExceedGasoline + amountExceedDiesel}", fontSize));
                    rows[5].Append(SetTableCell($"{amountExceedCO}", fontSize));
                    rows[6].Append(SetTableCell($"{amountExceedKSVOB}", fontSize));
                }

                var amountExceedGasolineTotal = carPostsDataAutoTest
                    .Where(c => c.MIN_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CO 
                    || c.MAX_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CO 
                    || c.MIN_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CH 
                    || c.MAX_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CH)
                    .Count();

                var amountExceedDieselTotal = carPostsDataSmokeMeter
                    .Where(c => c.K_SVOB > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_SVOB 
                    || c.K_MAX > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_MAX)
                    .Count();

                var amountExceedCOTotal = carPostsDataAutoTest
                    .Where(c => c.MIN_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CO 
                    || c.MAX_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CO)
                    .Count();

                var amountExceedKSVOBTotal = carPostsDataSmokeMeter
                    .Where(c => c.K_SVOB > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_SVOB)
                    .Count();

                rows[0].Append(SetTableCell($"Всего", fontSize));
                rows[1].Append(SetTableCell($"{carPostsDataAutoTest.Count() + carPostsDataSmokeMeter.Count()}", fontSize));
                rows[2].Append(SetTableCell($"{carPostsDataAutoTest.Count()}", fontSize));
                rows[3].Append(SetTableCell($"{carPostsDataSmokeMeter.Count()}", fontSize));
                rows[4].Append(SetTableCell($"{amountExceedGasolineTotal + amountExceedDieselTotal}", fontSize));
                rows[5].Append(SetTableCell($"{amountExceedCOTotal}", fontSize));
                rows[6].Append(SetTableCell($"{amountExceedKSVOBTotal}", fontSize));

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.OpenOrCreate)))
                {
                    sw.Write(docText);
                }
            }
            return report;
        }

        private Report CreateCarsExcessProtocol(Report report)
        {
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }
            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
            string reportFileNameFull = Path.Combine(userReportFolder, report.FileName);

            var typeEcoClasses = _context.TypeEcoClass.ToList();

            var carPostsDataAutoTest = _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.CarPost)
                .Where(c => report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .OrderBy(c => c.DateTime)
                .ToList();

            var carPostsDataSmokeMeter = _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.CarPost)
                .Where(c => report.CarPostStartDate <= c.DateTime && c.DateTime <= report.CarPostEndDate)
                .OrderBy(c => c.DateTime)
                .ToList();

            var amountExceedGasoline = carPostsDataAutoTest
                .Where(c => c.MIN_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CO 
                || c.MAX_CO > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CO 
                || c.MIN_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MIN_CH 
                || c.MAX_CH > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.MAX_CH);

            var amountExceedDiesel = carPostsDataSmokeMeter
                .Where(c => c.K_SVOB > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_SVOB 
                || c.K_MAX > typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(c.DOPOL2.ToLower()))?.K_MAX);

            var repeatedExceedancesGasoline = amountExceedGasoline
                .GroupBy(x => x.Number)
                .Where(g => g.Count() >= 2)
                .ToList();
            var repeatedExceedancesDiesel = amountExceedDiesel
                .GroupBy(x => x.Number)
                .Where(g => g.Count() >= 2)
                .ToList();

            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["StartDate"]}={Convert.ToDateTime(report.CarPostStartDate).ToString("yyyy-MM-dd")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["EndDate"]}={Convert.ToDateTime(report.CarPostEndDate).ToString("yyyy-MM-dd")};";

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

                docText = new Regex("StartDate").Replace(docText, report.CarPostStartDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + " г.");
                docText = new Regex("EndDate").Replace(docText, report.CarPostEndDate.Value.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + " г.");

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                Body body = wordDoc.MainDocumentPart.Document.Body;
                Table table = body.Elements<Table>().First();
                int fontSize = 24;
                for (int i = 0; i < repeatedExceedancesGasoline.Count; i++)
                {
                    table.Append(
                        new TableRow(
                            SetTableCell($"{repeatedExceedancesGasoline[i].Key}", fontSize),
                            SetTableCell($"{repeatedExceedancesGasoline[i].First().CarModelAutoTest.Name}", fontSize),
                            SetTableCell($"{repeatedExceedancesGasoline[i].First().DOPOL1}", fontSize),
                            SetTableCell($"{repeatedExceedancesGasoline[i].Count()}", fontSize),
                            SetTableCell($"{(repeatedExceedancesGasoline[i].First().CarModelAutoTest.EngineType == 1 ? "дизель" : "бензин")}", fontSize),
                            SetTableCell($"{repeatedExceedancesGasoline[i].First().CarModelAutoTest.CarPost.Name}", fontSize)));
            }
                for (int i = 0; i < repeatedExceedancesDiesel.Count; i++)
                {
                    table.Append(
                        new TableRow(
                            SetTableCell($"{repeatedExceedancesDiesel[i].Key}", fontSize),
                            SetTableCell($"{repeatedExceedancesDiesel[i].First().CarModelSmokeMeter.Name}", fontSize),
                            SetTableCell($"{repeatedExceedancesDiesel[i].First().DOPOL1}", fontSize),
                            SetTableCell($"{repeatedExceedancesDiesel[i].Count()}", fontSize),
                            SetTableCell($"{(repeatedExceedancesDiesel[i].First().CarModelSmokeMeter.EngineType == 1 ? "дизель" : "бензин")}", fontSize),
                            SetTableCell($"{repeatedExceedancesDiesel[i].First().CarModelSmokeMeter.CarPost.Name}", fontSize)));
            }

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }

            return report;
        }

        private Report CreateCarPostDataAutoTestProtocolPeriod(Report report)
        {
            List<string> reportNameFiles = new List<string>();
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }

            int carPostId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            List<CarPostDataAutoTest> carPostDataAutoTests = _context.CarPostDataAutoTest
                .Include(c => c.CarModelAutoTest)
                .Include(c => c.CarModelAutoTest.TypeEcoClass)
                .Include(c => c.CarModelAutoTest.CarPost)
                .Include(c => c.Tester)
                .Where(c => c.DateTime >= report.CarPostStartDate && c.DateTime <= report.CarPostEndDate && c.CarModelAutoTest.CarPostId == carPostId)
                .OrderBy(c => c.DateTime)
                .ToList();

            var typeEcoClasses = _context.TypeEcoClass.ToList();

            foreach (var carPostDataAutoTest in carPostDataAutoTests) 
            {
                var fileName = $"{carPostDataAutoTest.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
                string reportFileNameFull = Path.Combine(userReportFolder, fileName);
                reportNameFiles.Add(fileName);

                var carCheckNumber = _context.CarPostDataAutoTest
                    .Where(c => c.DateTime <= carPostDataAutoTest.DateTime && c.Number == carPostDataAutoTest.Number)
                    .Count()
                    .ToString();

                string reportTemplateFileNameFull = Path.Combine(Startup.Configuration["ReportsTeplatesFolder"].ToString(), report.NameRU.Replace(" за период", ""));
                reportTemplateFileNameFull = Path.ChangeExtension(reportTemplateFileNameFull, "docx");
                System.IO.File.Copy(reportTemplateFileNameFull, reportFileNameFull);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(reportFileNameFull, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    docText = new Regex("TestNumb").Replace(docText, carPostDataAutoTest.TestNumber.HasValue ? carPostDataAutoTest.TestNumber.Value.ToString() : string.Empty);
                    docText = new Regex("Day").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("dd"));
                    docText = new Regex("Month").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("MM"));
                    docText = new Regex("CarPostName").Replace(docText, carPostDataAutoTest.CarModelAutoTest.CarPost.Name);
                    docText = new Regex("Time").Replace(docText, carPostDataAutoTest.DateTime.Value.ToString("HH:mm:ss"));
                    docText = new Regex("GasSerialNumber").Replace(docText, carPostDataAutoTest.GasSerialNumber.HasValue ? carPostDataAutoTest.GasSerialNumber.Value.ToString() : string.Empty);
                    docText = new Regex("GasCheckDate").Replace(docText, carPostDataAutoTest.GasCheckDate.HasValue ? carPostDataAutoTest.GasCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                    docText = new Regex("MeteoSerialNumber").Replace(docText, carPostDataAutoTest.MeteoSerialNumber.HasValue ? carPostDataAutoTest.MeteoSerialNumber.Value.ToString() : string.Empty);
                    docText = new Regex("MeteoCheckDate").Replace(docText, carPostDataAutoTest.MeteoCheckDate.HasValue ? carPostDataAutoTest.MeteoCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                    docText = new Regex("Temperature").Replace(docText, carPostDataAutoTest.Temperature.HasValue ? carPostDataAutoTest.Temperature.Value.ToString() : string.Empty);
                    docText = new Regex("Pressure").Replace(docText, carPostDataAutoTest.Pressure.HasValue ? carPostDataAutoTest.Pressure.Value.ToString() : string.Empty);
                    docText = new Regex("CarModelName").Replace(docText, carPostDataAutoTest.CarModelAutoTest.Name);
                    docText = new Regex("CarNumber").Replace(docText, carPostDataAutoTest.Number);
                    docText = new Regex("Eco").Replace(docText, typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataAutoTest.DOPOL2.ToLower()))?.Name.Split(' ')[0]);
                    docText = new Regex("Cat").Replace(docText, carPostDataAutoTest.CarModelAutoTest.Category);
                    docText = new Regex("Check").Replace(docText, carCheckNumber);
                    docText = new Regex("CarYear").Replace(docText, carPostDataAutoTest.DOPOL1);
                    docText = new Regex("MIN_TAH").Replace(docText, carPostDataAutoTest.MIN_TAH.HasValue ? carPostDataAutoTest.MIN_TAH.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_TAH").Replace(docText, carPostDataAutoTest.MAX_TAH.HasValue ? carPostDataAutoTest.MAX_TAH.Value.ToString() : string.Empty);
                    docText = new Regex("MIN_CO_").Replace(docText, carPostDataAutoTest.MIN_CO.HasValue ? carPostDataAutoTest.MIN_CO.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_CO_").Replace(docText, carPostDataAutoTest.MAX_CO.HasValue ? carPostDataAutoTest.MAX_CO.Value.ToString() : string.Empty);
                    docText = new Regex("MIN_CH_").Replace(docText, carPostDataAutoTest.MIN_CH.HasValue ? carPostDataAutoTest.MIN_CH.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_CH_").Replace(docText, carPostDataAutoTest.MAX_CH.HasValue ? carPostDataAutoTest.MAX_CH.Value.ToString() : string.Empty);
                    docText = new Regex("MIN_CO2").Replace(docText, carPostDataAutoTest.MIN_CO2.HasValue ? carPostDataAutoTest.MIN_CO2.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_CO2").Replace(docText, carPostDataAutoTest.MAX_CO2.HasValue ? carPostDataAutoTest.MAX_CO2.Value.ToString() : string.Empty);
                    docText = new Regex("MIN_O2_").Replace(docText, carPostDataAutoTest.MIN_O2.HasValue ? carPostDataAutoTest.MIN_O2.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_O2_").Replace(docText, carPostDataAutoTest.MAX_O2.HasValue ? carPostDataAutoTest.MAX_O2.Value.ToString() : string.Empty);
                    docText = new Regex("MIN_NO_").Replace(docText, carPostDataAutoTest.MIN_NO.HasValue ? carPostDataAutoTest.MIN_NO.Value.ToString() : string.Empty);
                    docText = new Regex("MAX_NO_").Replace(docText, carPostDataAutoTest.MAX_NO.HasValue ? carPostDataAutoTest.MAX_NO.Value.ToString() : string.Empty);
                    docText = new Regex("Tester").Replace(docText, carPostDataAutoTest.Tester?.Name != null ? carPostDataAutoTest.Tester.Name.Split(' ')[0] : string.Empty);

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
            }

            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} c {report.CarPostStartDate.Value.ToString("dd-MM-yyyy")} по {report.CarPostEndDate.Value.ToString("dd-MM-yyyy")} (MS Word).zip";
            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostId"]}={carPostId};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostId"]}={carPostId};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostId"]}={carPostId};";

            if (report.PDF)
            {
                report.FileName = report.FileName.Replace("(MS Word)", "(PDF)");
                reportNameFiles = reportNameFiles.Select(f => f.Replace("(MS Word).docx", "(PDF).pdf")).ToList();

                foreach (var reportName in reportNameFiles)
                {
                    var fileName = reportName.Replace("(PDF).pdf", "(MS Word).docx");
                    ConvertDocToPdf(userReportFolder, reportName, fileName);
                }
            }

            using (var compressedFileStream = new FileStream(Path.Combine(userReportFolder, report.FileName), FileMode.CreateNew))
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    if (reportNameFiles.Count != 0) 
                    {
                        foreach (var reportNameFile in reportNameFiles)
                        {
                            var zipEntry = zipArchive.CreateEntry(reportNameFile);
                            var reportFileByte = System.IO.File.ReadAllBytes(Path.Combine(userReportFolder, reportNameFile));
                            using (var originalFileStream = new MemoryStream(reportFileByte))
                            {
                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    originalFileStream.CopyTo(zipEntryStream);
                                }
                            }
                        }
                    }
                    else
                    {
                        var zipEntry = zipArchive.CreateEntry("Нет данных.txt");
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            using (var streamWriter = new StreamWriter(zipEntryStream))
                            {
                                streamWriter.Write("Нет данных");
                            }
                        }
                    }
                }
            }
            reportNameFiles
                .Select(fileName => Path.Combine(userReportFolder, fileName))
                .ToList()
                .ForEach(System.IO.File.Delete);

            return report;
        }

        private Report CreateCarPostDataSmokeMeterProtocolPeriod(Report report)
        {
            List<string> reportNameFiles = new List<string>();
            string userReportFolder = Path.Combine(Startup.Configuration["ReportsFolder"].ToString(), report.ApplicationUserId);
            if (!Directory.Exists(userReportFolder))
            {
                Directory.CreateDirectory(userReportFolder);
            }

            int carPostId = Convert.ToInt32(report.Inputs.Split('=')[1]);
            List<CarPostDataSmokeMeter> carPostDataSmokeMeters = _context.CarPostDataSmokeMeter
                .Include(c => c.CarModelSmokeMeter)
                .Include(c => c.CarModelSmokeMeter.TypeEcoClass)
                .Include(c => c.CarModelSmokeMeter.CarPost)
                .Include(c => c.Tester)
                .Where(c => c.DateTime >= report.CarPostStartDate && c.DateTime <= report.CarPostEndDate && c.CarModelSmokeMeter.CarPostId == carPostId)
                .OrderBy(c => c.DateTime)
                .ToList();

            var typeEcoClasses = _context.TypeEcoClass.ToList();

            foreach (var carPostDataSmokeMeter in carPostDataSmokeMeters)
            {
                var fileName = $"{carPostDataSmokeMeter.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} (MS Word).docx";
                string reportFileNameFull = Path.Combine(userReportFolder, fileName);
                reportNameFiles.Add(fileName);

                var carCheckNumber = _context.CarPostDataSmokeMeter
                    .Where(c => c.DateTime <= carPostDataSmokeMeter.DateTime && c.Number == carPostDataSmokeMeter.Number)
                    .Count()
                    .ToString();

                string reportTemplateFileNameFull = Path.Combine(Startup.Configuration["ReportsTeplatesFolder"].ToString(), report.NameRU.Replace(" за период", ""));
                reportTemplateFileNameFull = Path.ChangeExtension(reportTemplateFileNameFull, "docx");
                System.IO.File.Copy(reportTemplateFileNameFull, reportFileNameFull);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(reportFileNameFull, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    docText = new Regex("TestNumb").Replace(docText, carPostDataSmokeMeter.TestNumber.HasValue ? carPostDataSmokeMeter.TestNumber.Value.ToString() : string.Empty);
                    docText = new Regex("Day").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("dd"));
                    docText = new Regex("Month").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("MM"));
                    docText = new Regex("CarPostName").Replace(docText, carPostDataSmokeMeter.CarModelSmokeMeter.CarPost.Name);
                    docText = new Regex("Time").Replace(docText, carPostDataSmokeMeter.DateTime.Value.ToString("HH:mm:ss"));
                    docText = new Regex("GasSerialNumber").Replace(docText, carPostDataSmokeMeter.GasSerialNumber.HasValue ? carPostDataSmokeMeter.GasSerialNumber.Value.ToString() : string.Empty);
                    docText = new Regex("GasCheckDate").Replace(docText, carPostDataSmokeMeter.GasCheckDate.HasValue ? carPostDataSmokeMeter.GasCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                    docText = new Regex("MeteoSerialNumber").Replace(docText, carPostDataSmokeMeter.MeteoSerialNumber.HasValue ? carPostDataSmokeMeter.MeteoSerialNumber.Value.ToString() : string.Empty);
                    docText = new Regex("MeteoCheckDate").Replace(docText, carPostDataSmokeMeter.MeteoCheckDate.HasValue ? carPostDataSmokeMeter.MeteoCheckDate.Value.ToString("dd.MM.yyyy") : string.Empty);
                    docText = new Regex("Temperature").Replace(docText, carPostDataSmokeMeter.Temperature.HasValue ? carPostDataSmokeMeter.Temperature.Value.ToString() : string.Empty);
                    docText = new Regex("Pressure").Replace(docText, carPostDataSmokeMeter.Pressure.HasValue ? carPostDataSmokeMeter.Pressure.Value.ToString() : string.Empty);
                    docText = new Regex("CarModelName").Replace(docText, carPostDataSmokeMeter.CarModelSmokeMeter.Name);
                    docText = new Regex("CarNumber").Replace(docText, carPostDataSmokeMeter.Number);
                    docText = new Regex("Check").Replace(docText, carCheckNumber);
                    docText = new Regex("Eco").Replace(docText, typeEcoClasses.FirstOrDefault(t => t.Name.ToLower().Contains(carPostDataSmokeMeter.DOPOL2.ToLower()))?.Name.Split(' ')[0]);
                    docText = new Regex("Cat").Replace(docText, carPostDataSmokeMeter.CarModelSmokeMeter.Category);
                    docText = new Regex("CarYear").Replace(docText, carPostDataSmokeMeter.DOPOL1);
                    docText = new Regex(@"\b(DFree)\b").Replace(docText, carPostDataSmokeMeter.K_SVOB.HasValue ? carPostDataSmokeMeter.K_SVOB.Value.ToString() : string.Empty);
                    docText = new Regex(@"\b(NDFree)\b").Replace(docText, carPostDataSmokeMeter.K_MAX.HasValue ? carPostDataSmokeMeter.K_MAX.Value.ToString() : string.Empty);
                    docText = new Regex("Tester").Replace(docText, carPostDataSmokeMeter.Tester?.Name != null ? carPostDataSmokeMeter.Tester.Name.Split(' ')[0] : string.Empty);

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
            }

            report.FileName = $"{report.DateTime.Value.ToString("yyyy-MM-dd HH.mm.ss")} {report.Name} c {report.CarPostStartDate.Value.ToString("dd-MM-yyyy")} по {report.CarPostEndDate.Value.ToString("dd-MM-yyyy")} (MS Word).zip";
            report.InputParametersEN = $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("en"))["CarPostId"]}={carPostId};";
            report.InputParametersRU = $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("ru"))["CarPostId"]}={carPostId};";
            report.InputParametersKK = $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostStartDate"]}={report.CarPostStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostEndDate"]}={report.CarPostEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss")};" +
                $"{_sharedLocalizer.WithCulture(new CultureInfo("kk"))["CarPostId"]}={carPostId};";

            if (report.PDF)
            {
                report.FileName = report.FileName.Replace("(MS Word)", "(PDF)");
                reportNameFiles = reportNameFiles.Select(f => f.Replace("(MS Word).docx", "(PDF).pdf")).ToList();

                foreach (var reportName in reportNameFiles)
                {
                    var fileName = reportName.Replace("(PDF).pdf", "(MS Word).docx");
                    ConvertDocToPdf(userReportFolder, reportName, fileName);
                }
            }

            using (var compressedFileStream = new FileStream(Path.Combine(userReportFolder, report.FileName), FileMode.CreateNew))
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    if (reportNameFiles.Count != 0)
                    {
                        foreach (var reportNameFile in reportNameFiles)
                        {
                            var zipEntry = zipArchive.CreateEntry(reportNameFile);
                            var reportFileByte = System.IO.File.ReadAllBytes(Path.Combine(userReportFolder, reportNameFile));
                            using (var originalFileStream = new MemoryStream(reportFileByte))
                            {
                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    originalFileStream.CopyTo(zipEntryStream);
                                }
                            }
                        }
                    }
                    else
                    {
                        var zipEntry = zipArchive.CreateEntry("Нет данных.txt");
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            using (var streamWriter = new StreamWriter(zipEntryStream))
                            {
                                streamWriter.Write("Нет данных");
                            }
                        }
                    }
                }
            }
            reportNameFiles
                .Select(fileName => Path.Combine(userReportFolder, fileName))
                .ToList()
                .ForEach(System.IO.File.Delete);

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

        private void ConvertDocToPdf(string pathDoc, string nameDoc, string wordName)
        {
            string YourApplicationPath = Startup.Configuration["ReportsFolder"].ToString();
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processInfo.FileName = "cmd.exe";
            processInfo.WorkingDirectory = YourApplicationPath;
            processInfo.Arguments = $"/c OfficeToPDF.exe \"{Path.Combine(pathDoc, wordName)}\" \"{Path.Combine(pathDoc, nameDoc)}\"";
            Process.Start(processInfo).WaitForExit();

            if (System.IO.File.Exists(Path.Combine(pathDoc, wordName)))
            {
                System.IO.File.Delete(Path.Combine(pathDoc, wordName));
            }
        }

        private bool ReportExists(int id)
        {
            return _context.Report.Any(e => e.Id == id);
        }

        private TableCell SetTableCell(string text, int fontSize)
        {
            TableCell tableCell = new TableCell();

            //create the cell properties
            TableCellProperties tcp = new TableCellProperties();
            //create the vertial alignment properties
            TableCellVerticalAlignment tcVA = new TableCellVerticalAlignment() 
            { 
                Val = TableVerticalAlignmentValues.Center
            };
            tcp.Append(tcVA);
            tableCell.Append(tcp);
            tableCell.Append(SetParagraphStyle(text, fontSize));

            return tableCell;
        }

        private Paragraph SetParagraphStyle(string text, int fontSize)
        {
            //paragraph properties 
            ParagraphProperties User_heading_pPr = new ParagraphProperties();
            //trying to align center a paragraph
            Justification justification = new Justification() { Val = JustificationValues.Center };
            User_heading_pPr.Append(justification);

            RunProperties runProperties = new RunProperties()
            {
                RunFonts = new RunFonts()
                {
                    Ascii = "Times New Roman",
                    HighAnsi = "Times New Roman",
                    ComplexScript = "Times New Roman"
                },
                FontSize = new FontSize()
                {
                    Val = fontSize.ToString()
                }
            };

            Run run = new Run(runProperties);
            run.Append(new Text($"{text}"));

            Paragraph paragraph = new Paragraph(User_heading_pPr);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
