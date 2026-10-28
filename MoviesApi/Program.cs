using MoviesApi.Data;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Services;
namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // requires Swashbuckle.AspNetCore package
            // Add CORS
            //builder.Services.AddCors();
            builder.Services.AddTransient<IGenresService,GenresService>();
            builder.Services.AddTransient<IMoviesService, MoviesService>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            // Enable CORS
            //app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
