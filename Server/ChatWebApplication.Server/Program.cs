
namespace ChatWebApplication.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(o => {o.AddPolicy("AllowAnyOrigin", p => p
                .WithOrigins("null") // Origin of an html file opened in a browser
                .AllowAnyHeader()
                .AllowCredentials());
            });
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseCors("AllowAnyOrigin");
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
