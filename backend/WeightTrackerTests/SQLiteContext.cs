using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightTracker.Data;

namespace WeightTrackerTests
{
    public class SQLiteContext
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<WeightTrackerDbContext> _contextOptions;
        private readonly WeightTrackerDbContext _context;

        public SQLiteContext()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<WeightTrackerDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new WeightTrackerDbContext(_contextOptions);
            _context.Database.EnsureCreated();
        }

        public WeightTrackerDbContext CreateSQLiteContext() => _context;
    }
}
