
using ECommerce.API.Data;
using ECommerce.API.Repositories.IRepositories;
using ECommerce.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Mapster;
using System.Reflection;
using MapsterMapper;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            builder.Services.AddSingleton<IMapper>(new Mapper(config));

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            //app.MapIdentityApi<ApplicationUser>();
            app.MapControllers();

            app.Run();
        }
    }
}
