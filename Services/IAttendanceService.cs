using FarmOps.Models.AttendanceModels;
using System.Data;

namespace FarmOps.Services
{
    public interface IAttendanceService
    {
        public string SaveTimeAttendance(List<TimeAttendanceInsert> data);

        public List<string> GetRelatedWorkers (string UserType, string UserId);

        public List<ModelAttendanceViewRecord> GetAllAttendanceData(string UserType, string UserId);
    }
}
