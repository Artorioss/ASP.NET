using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;
using PromoCodeFactory.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(Context context)
        {
            //await context.Database.EnsureDeletedAsync();
            //await context.Database.MigrateAsync();

            // 1. Добавляем Roles
            var roles = FakeDataFactory.Roles.ToList();
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            // 2. Добавляем Employees
            var employees = FakeDataFactory.Employees.ToList();

            // Важно: получаем роли из базы, а не из исходной коллекции
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var partnerRole = await context.Roles.FirstAsync(r => r.Name == "PartnerManager");

            employees[0].Role = adminRole;
            employees[1].Role = partnerRole;

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // 3. Добавляем Preferences
            await context.Preferences.AddRangeAsync(FakeDataFactory.Preferences);
            await context.SaveChangesAsync();

            // 4. Добавляем Customers
            var customers = FakeDataFactory.Customers.ToList();
            var preferences = await context.Preferences.ToListAsync();

            customers[0].Preferences = new List<Preference>
            {
                preferences.First(p => p.Name == "Театр"),
                preferences.First(p => p.Name == "Семья")
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }
    }
}
