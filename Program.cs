using Todo_List.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;
using System.Text;

namespace Todo_List
{

    public class Programs
    {

        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDBContext>(options =>
                options.UseSqlite("Filename=database/todo.db")
            );

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                                      policy =>
                                      {
                                          policy.AllowAnyOrigin()
                                                              .AllowAnyHeader()
                                                              .AllowAnyMethod();
                                                              
                                      });
            });

            // Add services to the container.
            var jwtIssuer = builder.Configuration.GetSection("JWT:ValidIssuer").Get<string>();
            var jwtKey = builder.Configuration.GetSection("Jwt:Secret").Get<string>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtIssuer,
                     ValidAudience = jwtIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
             });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);
            // Add JWT authentication
            


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }

}
