using System.ComponentModel.DataAnnotations;

namespace CalendarAPI
{
    public class WorkDay
    {
        [Key]
        public Guid Id { get; set; }
        public List<WorkTime> ListOfWorktime {  get; set; }

        public DateOnly Today { get; set; }

        public WorkDay()
        {
            Id = Guid.NewGuid();  // Se genereaza un nou GUID la fiecare instanta
        }


    }
}
