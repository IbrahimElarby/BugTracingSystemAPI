

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugProject.BL
{
    public static class BussinessExtentions
    {
        public static void AddBusinessServices(
       this IServiceCollection services)
        {
           
            
           services.AddScoped<IBugManger, BugManger>();
           services.AddScoped<IProjectManger, ProjectManger>();
            services.AddScoped<IAccountManager, AccountManager>();




        }
    }
}
