using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BugProject.DL
{
    public static class DataAccessExtention
    {
        public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("default");
            services.AddDbContext<BugContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IProjectRepository,ProjectRepository>();
            services.AddScoped<IBugRepository,BugRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

        }
    }
}
