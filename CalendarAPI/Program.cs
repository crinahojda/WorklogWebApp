using Microsoft.EntityFrameworkCore;

namespace CalendarAPI
{

    public class Program
    {
       
        const string PATH_PREFIX = "/api";

        const string DEV_POLICY_NAME = "DevelopmentPolicy";


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Adaugam o baza de date sql ca serviciu al aplicatiei noastre printr-un string de conexiune
            builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer("Server=(localdb)\\WorkLogDb; Initial Catalog=WorkLogDb; Integrated Security=True;"));

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("Content-Disposition");
                });
            });

            var app = builder.Build();

    
            using (IServiceScope scope = app.Services.CreateScope())
            {
                using (AppDbContext dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                {
                    
                    dbContext.Database.Migrate(); //facem migrarile in baza de date

                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UsePathBase(new PathString(PATH_PREFIX));
            app.UseCors("AllowAll");
            app.MapControllers();        
            app.Run();

        }



    }

}

