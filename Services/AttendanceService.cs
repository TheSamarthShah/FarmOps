using FarmOps.Common;
using FarmOps.Controllers;
using FarmOps.Models.AttendanceModels;
using FarmOps.Repos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace FarmOps.Services
{
    public class AttendanceService 
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly string _attendanceSignsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AppData", "AttendanceSigns");
        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }
        public string SaveTimeAttendance(List<TimeAttendanceInsert> data)
        {
            List<FAttendanceTbl> AttendanceTbl = data.Select(item =>{
                string attendanceSignPicPath = SaveAttendanceSignPic(item);
                return new FAttendanceTbl
                {
                    RosterId = item.RosterId,
                    AttendanceDate = item.AttendanceDate,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    TotalHours = item.TotalHours,
                    BreakTime = item.BreakTime,
                    BlockId = item.BlockId,
                    Pay = item.Pay,
                    AttendanceType = "Time", // Assuming you want to leave this null as it's not part of TimeAttendanceInsert model
                    JobId = null, // Assuming no mapping for JobId, if needed, you can adjust this
                    PaidBreak = null, // Assuming no mapping for PaidBreak, if needed, you can adjust this
                    AttendanceSignPic = attendanceSignPicPath,
                    LineId = item.LineId,
                    JobPaid = item.JobPaid,
                    Remarks = item.Remarks,
                    AppliedBy = item.AppliedBy,
                    ApprovedStatus = item.ApprovedStatus, // Default to 0 if null
                    ApprovedBy = item.ApprovedBy,
                    ApprovedDt = item.ApprovedDt
                };
            }).ToList();

            _attendanceRepository.SaveAppliedAttendance(AttendanceTbl);

            return "success";
        }
        // Method to save base64 string as a PNG file
        private string SaveAttendanceSignPic(TimeAttendanceInsert item)
        {
            if (string.IsNullOrEmpty(item.AttendanceSignPic))
            {
                return null; // No picture to save
            }

            try
            {
                // Create the AttendanceSigns directory if it doesn't exist
                if (!Directory.Exists(_attendanceSignsDirectory))
                {
                    Directory.CreateDirectory(_attendanceSignsDirectory);
                }

                // Decode the base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(item.AttendanceSignPic);

                // Generate the file name based on RosterId, AttendanceDate, StartTime, and EndTime
                string fileName = $"{item.RosterId}_{item.AttendanceDate:yyyyMMdd}_{item.StartTime:hhmm}_{item.EndTime:hhmm}.png";
                string filePath = Path.Combine(_attendanceSignsDirectory, fileName);

                // Save the byte array as a PNG file
                File.WriteAllBytes(filePath, imageBytes);

                return fileName; // Return the saved file path
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error saving AttendanceSignPic: {ex.Message}");
                return null; // Return null if an error occurs
            }
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
