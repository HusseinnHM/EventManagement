using System;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Entities;
using EventManagement.Infrastructure.EntityFrameworkCore.DbContexts;
using EventManagement.Infrastructure.PasswordHash;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Infrastructure.EntityFrameworkCore;

public class DataSeed
{
    public static async Task Seed(EventManagementDbContext context, IServiceProvider serviceProvider)
    {
        await SeedUsers(context,serviceProvider.GetRequiredService<IPasswordHasher>());
        
    }
    

    private static async Task SeedUsers(EventManagementDbContext context,IPasswordHasher passwordHasher)
    {
        if (context.ParticipationUsers.Any())
        {
            return;
        }

        context.ParticipationUsers.Add(new ParticipationUser("Seed participation", "seed@test.com", passwordHasher.HashPassword("1234")));
        context.EventManagers.Add(new EventManager("Seed EventManager", "seed@test.com", passwordHasher.HashPassword("1234")));
       
        await context.SaveChangesAsync();
    }   
    
}