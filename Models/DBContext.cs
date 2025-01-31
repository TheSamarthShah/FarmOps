using FarmOps.Models.AttendanceModels;
using FarmOps.Models.LoginModels;
using Microsoft.EntityFrameworkCore;

namespace FarmOps.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options): base(options) { 
            
        }
        public DbSet<LoginModel> LoginModels { get; set; }
        public DbSet<AppliedAttendanceTblModel> appliedAttendanceTblModels { get; set; }
        public DbSet<AttendanceDetailTblModel> attendanceDetailTblModels { get; set; }
    }
}
