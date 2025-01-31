namespace FarmOps.Models.AttendanceModels
{
    public class ModelAttendanceViewRecord
    {
        public long? AttendanceId { get; set; }
        public long RosterId { get; set; }
        public DateTime attendDate { get; set; }
        public TimeSpan? SigninTime { get; set; }  // Using TimeSpan to store time (nullable)
        public TimeSpan? SignoutTime { get; set; }
        public decimal? TotalWorkHours { get; set; }
        public decimal? TotalBreakHours { get; set; }
        public string? BreakIds { get; set; }
        public decimal? Pay { get; set; }
        public int? JobCat { get; set; }
        public bool? OnPaidLeave { get; set; }
        public int? LineId { get; set; }
        public bool? JobNotPaid { get; set; }
        public string? ApprovedStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public string? AttendanceSheetPhoto { get; set; }
        public string? RejectedRosterIds { get; set; }
        public DateTime? ApprovedDt { get; set; }
        public string? AppliedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
