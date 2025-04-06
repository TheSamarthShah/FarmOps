using FarmOps.Models.AttendanceModels;
using System.Data;

namespace FarmOps.Repos
{
    public interface IAttendanceRepository
    {
        public string SaveAppliedAttendance(List<FAttendanceTbl> data);
        public string SaveAttendanceDetails(AttendanceDetailTblModel data);

        public DataTable GetAssignedWorkers(string ClmName, string UserId);

        public DataTable GetAllAttendanceData(string UserType, string UserId);
        public long GetNextAttendanceId();
    }
}
