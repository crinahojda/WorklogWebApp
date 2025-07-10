using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkTimeController : ControllerBase
    {

            private readonly AppDbContext _context;
            private readonly ILogger<WorkTimeController> _logger;

            public WorkTimeController(AppDbContext context, ILogger<WorkTimeController> logger)
            {
                _context = context;
                _logger = logger;
            }
        //adaugare timp unui worker
            [HttpPost]
            [Route("[action]")]
            public async Task<IActionResult> AddWorkTimeWithDetails([FromBody] WorkTime workTime)
            {
                var worker = await _context.Workers.FirstOrDefaultAsync(w => w.Id == workTime.WorkerGuid);
                 if (worker == null)
                    return NotFound("Worker not found.");

                _context.WorkTimes.Add(workTime);
                await _context.SaveChangesAsync();

                return Ok(workTime);
            }

        //returneaza timpii unui worker

            [HttpGet]
            [Route("[action]")]
            public async Task<IActionResult> GetWorkTimesByWorker(Guid workerId)
            {
            var workTimes = await _context.WorkTimes
                .Where(wt => wt.WorkerGuid == workerId)
                .ToListAsync();

            return Ok(workTimes);
            }


        //modifica timpii unui worker
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateWorkTime(Guid id, [FromBody] WorkTime updatedWorkTime)
        {
            var existingWorkTime = await _context.WorkTimes.FindAsync(id);
            if (existingWorkTime == null)
                return NotFound("Timpul de lucru nu a fost găsit.");

            existingWorkTime.StartDate = updatedWorkTime.StartDate;
            existingWorkTime.EndDate = updatedWorkTime.EndDate;
            existingWorkTime.Description = updatedWorkTime.Description;

            await _context.SaveChangesAsync();

            return Ok(existingWorkTime);
        }

        //sterge timpii unui worker
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteWorkTime(Guid id)
        {
            var workTime = await _context.WorkTimes.FindAsync(id);
            if (workTime == null)
                return NotFound("Timpul de lucru nu a fost găsit.");

            _context.WorkTimes.Remove(workTime);
            await _context.SaveChangesAsync();

            return Ok("Timpul de lucru a fost șters cu succes.");
        }

        //genereaza raport worker
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GenerateReportForWorker(Guid workerId)
        {

            List<ReportWorkerModel> records = new List<ReportWorkerModel>();
            List<WorkTime> workTimes = await _context.WorkTimes.Where(x => x.WorkerGuid == workerId).OrderBy(x => x.StartDate).ToListAsync();

            if (workTimes.Count == 0)
                return Problem();

            string workerName = (await _context.Workers.Where(x => x.Id == workerId).FirstOrDefaultAsync() as Worker).Name;

            MemoryStream memoryStream = new MemoryStream(); //mod de salvare in memorie la run time a unui sir de biti
            StreamWriter streamWriter = new StreamWriter(memoryStream, leaveOpen: true); //se foloseste de memory stream ptr a scrie
            CsvWriter csv = new CsvHelper.CsvWriter(streamWriter, CultureInfo.InvariantCulture); //clasa de scriere e unui fisier csv din NuGet csv helper
            
            foreach (WorkTime workTime in workTimes)
            {
                records.Add(new ReportWorkerModel
                {
                    EndDate = workTime.EndDate,
                    StartDate = workTime.StartDate,
                    Length = (float)(workTime.EndDate - workTime.StartDate).TotalHours   // diferenta intre 2 obiecte de tip date time este de tipul TimeSpan - care contine timpul in ore - .NET
                });
            }

            csv.WriteRecords(records);

            streamWriter.Flush(); //porneste scrierea de biti efectiv
            memoryStream.Position = 0;  // Resetăm poziția streamului ptr. a fi citit ulterior
    
            var fileName = $"{workerName}_report.csv";
            return File(memoryStream, "text/csv", fileName);
        }
    }
}
