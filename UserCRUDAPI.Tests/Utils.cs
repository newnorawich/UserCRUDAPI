using Infranstructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCRUDAPI.Tests
{
    public class Utils
    {
        public static UserDbContext setUpDatabase(string database)
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
              .UseSqlite("Data Source = " + database).Options;
            var context = new UserDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
    }
}
