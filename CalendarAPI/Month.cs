using System.ComponentModel.DataAnnotations;

namespace CalendarAPI
{
    public class Month
    {
        [Key]
        public Guid Id { get; set; }  // Proprietate - cu GUID unic
        public List<WorkDay> Days { get; set; }

        public Month()
        {
            Id = Guid.NewGuid();  // Se genereaza un nou GUID la fiecare instanta
        }

    }
}
