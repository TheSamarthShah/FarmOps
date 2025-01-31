using FarmOps.Common;
using FarmOps.Models.AttendanceModels;
using FarmOps.Repos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace FarmOps.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }
        public string SaveAttendance(AttendaceInsertModel data)
        {
            List<AppliedAttendanceTblModel> AppliedData = new List<AppliedAttendanceTblModel>();
            AttendanceDetailTblModel AttendanceDetails = new AttendanceDetailTblModel();
            AppliedData = data.AttendanceRecords.Select(record => new AppliedAttendanceTblModel
            {
                RosterID = record.RosterID,
                SigninTime = string.IsNullOrEmpty(record.SignInTime) ? (TimeOnly?)null : TimeOnly.Parse(record.SignInTime),
                SignoutTime = string.IsNullOrEmpty(record.SignOutTime) ? (TimeOnly?)null : TimeOnly.Parse(record.SignOutTime),
                Pay = record.Pay ?? 0, // If Pay is null, you can set it to a default value (e.g., 0)
                OnPaidLeave = record.OnPaidLeave,
                JobNotPaid = record.JobNotPaid,

                // Setting other properties to default or null values
                AttendanceId = _attendanceRepository.GetNextAttendanceId(),   // Nullable
                TotalWorkHours = null, // Nullable
                TotalBreakHours = null, // Nullable
                BreakIds = null,       // Nullable
                JobCat = 0,            // Default value, you can change it accordingly
                LineId = 0,            // Default value, you can change it accordingly
                InsertId = null,       // Nullable
                InsertDt = DateTime.Now,  // Default to current date and time
                UpdtId = null,         // Nullable
                UpdtDt = null,         // Nullable
                AttendanceDate = data.AttendanceDate // Default to current date
            }).ToList();

            _attendanceRepository.SaveAppliedAttendance(AppliedData);

            return "success";
        }

        public List<string> GetRelatedWorkers(string UserType, string UserId)
        {
            var clmName = "supervisorId";
            if (UserType == "M")
            {
                clmName = "MonitorId";
            }
            DataTable dutyTbl = _attendanceRepository.GetAssignedWorkers(clmName, UserId);
            GlobalVariables.DutyTable = dutyTbl;

            HashSet<string> uniqueWorkerIds = new HashSet<string>();
            foreach (DataRow row in dutyTbl.Rows)
            {
                uniqueWorkerIds.Add(row.Field<string>("WorkerID").ToString());
            }
            return uniqueWorkerIds.ToList();
        }

        public List<ModelAttendanceViewRecord> GetAllAttendanceData(string UserType, string UserId)
        {
            var clmName = "supervisorId";
            if (UserType == "M")
            {
                clmName = "MonitorId";
            }
            DataTable attendanceDT = _attendanceRepository.GetAllAttendanceData(clmName, UserId);
            var records = new List<ModelAttendanceViewRecord>();

            foreach (DataRow row in attendanceDT.Rows)
            {
                var record = new ModelAttendanceViewRecord
                {
                    AttendanceId = row.Field<long>("AttendanceId"),
                    RosterId = row.Field<long>("RosterId"),
                    attendDate = (DateTime)(row.IsNull("attendDate") ? (DateTime?)null : row.Field<DateTime>("attendDate")),
                    SigninTime = row.IsNull("SigninTime") ? (TimeSpan?)null : row.Field<TimeSpan>("SigninTime"),
                    SignoutTime = row.IsNull("SignoutTime") ? (TimeSpan?)null : row.Field<TimeSpan>("SignoutTime"),
                    TotalWorkHours = row.IsNull("TotalWorkHours") ? (decimal?)null : row.Field<decimal>("TotalWorkHours"),
                    TotalBreakHours = row.IsNull("TotalBreakHours") ? (decimal?)null : row.Field<decimal>("TotalBreakHours"),
                    BreakIds = row.Field<string>("BreakIds"),
                    Pay = row.IsNull("Pay") ? (decimal?)null : row.Field<decimal>("Pay"),
                    JobCat = row.IsNull("job_cat") ? (int?)null : row.Field<int>("job_cat"),
                    OnPaidLeave = row.IsNull("on_paid_leave") ? (bool?)null : row.Field<bool>("on_paid_leave"),
                    LineId = row.IsNull("LineId") ? (int?)null : row.Field<int>("LineId"),
                    JobNotPaid = row.IsNull("JobNotPaid") ? (bool?)null : row.Field<bool>("JobNotPaid"),
                    ApprovedStatus = row.Field<string>("ApprovedStatus"),
                    ApprovedBy = row.Field<string>("ApprovedBy"),
                    AttendanceSheetPhoto = row.Field<string>("AttendanceSheetPhoto"),
                    RejectedRosterIds = row.Field<string>("RejectedRosterIds"),
                    ApprovedDt = row.IsNull("ApprovedDt") ? (DateTime?)null : row.Field<DateTime>("ApprovedDt"),
                    AppliedBy = row.Field<string>("AppliedBy"),
                    Remarks = row.Field<string>("Remarks")
                };

                records.Add(record);
            }
            return records;
        }
    }
}
