using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IntelliTect.TestTools.AspNetCore
{
    static public class ServiceProvider
    {
        static public TDbContext GetDbContext<TDbContext>(this IServiceProvider serviceProvider)
            where TDbContext: DbContext
        {
            return serviceProvider.GetRequiredService<TDbContext>();
        }

        static public RoleManager<IdentityRole> GetRoleManager(this IServiceProvider serviceProvider)
        {
            return (RoleManager<IdentityRole>)serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //return new RoleManager<IdentityRole>(
            //    new RoleStore<IdentityRole, TIdentityDbContext>(database), new IRoleValidator<IdentityRole>[0],
            //    new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Logger<RoleManager<IdentityRole>>(new LoggerFactory()),
            //    new HttpContextAccessor()
            //    );
        }

        static public UserManager<TIdentityUser> GetUserManager<TIdentityUser>(this IServiceProvider serviceProvider)
            where TIdentityUser: class
        {
            return serviceProvider.GetRequiredService<UserManager<TIdentityUser>>();
            
            //return new UserManager<TIdentityUser>(
            //    new UserStore<TIdentityUser>(database),
            //    new OptionsManager<IdentityOptions>(Enumerable.Empty<IConfigureOptions<IdentityOptions>>()),
            //    new PasswordHasher<TIdentityUser>(),
            //    new IUserValidator<TIdentityUser>[0],
            //    new IPasswordValidator<TIdentityUser>[0],
            //    new UpperInvariantLookupNormalizer(),
            //    new IdentityErrorDescriber(),
            //    new ServiceCollection().BuildServiceProvider(),
            //    new Logger<UserManager<TIdentityUser>>(new LoggerFactory()));
        }
    }

    public class TestServicesCollection<TIdentityDbContext, TIdentityUser>
        where TIdentityDbContext : IdentityDbContext<TIdentityUser>
        where TIdentityUser : IdentityUser, new()
    {
        static public IServiceProvider CreateServiceProvider(string sqlServerConnectionString, IServiceCollection services = null)
        {
            services = services ?? new ServiceCollection();
            var blah = CreateServiceProvider(option => option.UseSqlServer(sqlServerConnectionString), services);

            return blah;
        }
        static public IServiceProvider CreateServiceProvider(Action<DbContextOptionsBuilder> dbContextOptionsAction)
        {
            return CreateServiceProvider(dbContextOptionsAction, new ServiceCollection());
        }

        static public IServiceProvider CreateServiceProvider(Action<DbContextOptionsBuilder> dbContextOptionsAction, IServiceCollection services)
        {
            services.AddDbContext<TIdentityDbContext>(dbContextOptionsAction);

            services.AddIdentity<TIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddDefaultTokenProviders();
            services.AddLogging();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddIdentity<TIdentityUser, IdentityRole>();

            return services.BuildServiceProvider();
        }

     }
}