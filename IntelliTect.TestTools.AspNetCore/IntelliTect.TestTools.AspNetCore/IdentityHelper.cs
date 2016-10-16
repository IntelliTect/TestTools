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

namespace IntelliTect.TestTools.AspNetCore
{
    public static class IdentityHelper
    {
        static public RoleManager<IdentityRole> CreateRoleManager<TIdentityDbContext, TIdentityUser>(TIdentityDbContext database) 
            where TIdentityDbContext : IdentityDbContext<TIdentityUser>
            where TIdentityUser : IdentityUser, new()
        {
            return new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole, TIdentityDbContext>(database), new IRoleValidator<IdentityRole>[0],
                new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Logger<RoleManager<IdentityRole>>(new LoggerFactory()),
                new HttpContextAccessor()
                );
        }

        static public UserManager<TIdentityUser> CreateUserManager<TIdentityDbContext, TIdentityUser>(TIdentityDbContext database) 
            where TIdentityDbContext : IdentityDbContext<TIdentityUser>
            where TIdentityUser : IdentityUser, new()
        {
            return new UserManager<TIdentityUser>(
                new UserStore<TIdentityUser>(database),
                new OptionsManager<IdentityOptions>(Enumerable.Empty<IConfigureOptions<IdentityOptions>>()),
                new PasswordHasher<TIdentityUser>(),
                new IUserValidator<TIdentityUser>[0],
                new IPasswordValidator<TIdentityUser>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<TIdentityUser>>(new LoggerFactory()));
        }
    }
}