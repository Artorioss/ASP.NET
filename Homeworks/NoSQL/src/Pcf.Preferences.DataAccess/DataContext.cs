using Microsoft.EntityFrameworkCore;
using Pcf.Preferences.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.Preferences.DataAccess
{
    public class DataContext: DbContext
    {
        public DbSet<Preference> Preferences { get; set; }

        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
