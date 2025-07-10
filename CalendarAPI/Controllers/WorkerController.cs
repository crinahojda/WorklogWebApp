using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(AppDbContext context, ILogger<WorkerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task AddWorker()
        {
            Worker worker = new Worker();
            worker.Email = "andreeacrina17@ceva.com";
            worker.Name = "Andreea Popescu";
            worker.Position = "Software Developer";

            _context.Workers.Add(worker);

            worker = new Worker();
            worker.Email = "ionut123@ceva.com";
            worker.Name = "Ionut Pop";
            worker.Position = "HR Specialist";

            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();
            return;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task UpdateWorker(string newRole)
        {
            Worker worker = _context.Workers.ToList().First();
            worker.Position = newRole;
            _context.Update(worker);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        [Route("[action]")]

        public async Task<IActionResult> GetWorkers()
        {
            List<Worker> workers = _context.Workers.ToList();

            return Ok(workers.ToJson());
        }

         

    }
}
