using System.ComponentModel.DataAnnotations;

namespace CalendarAPI
{
    public class WorkTime
    {
        // propritetate cu GUID unic

        [Key]
        public Guid Id { get; set; }  // Proprietate cu GUID unic

       
        // membrii: 1. worker, 2. startdate and enddate de tipul DateTime

        //proprietate Guid care referentiaza la worker
        public Guid WorkerGuid { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string? Description { get; set; }

   
        public WorkTime()
        {
            Id = Guid.NewGuid();  // Se generează un nou GUID la fiecare instanta
        }

    }
}
