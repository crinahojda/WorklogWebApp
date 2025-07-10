using System.ComponentModel.DataAnnotations;
using CalendarAPI;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI
{
    public class Worker
    {
        [Key]
        public Guid Id { get; set; }  // Proprietate cu GUID unic
        public string Email { get; set; }   
        public string Name { get; set; }
        public string Position { get; set; }
     
        public Worker()
        {
            Id = Guid.NewGuid(); // Se genereaza un nou GUID la fiecare instanta
        }
    }


}
