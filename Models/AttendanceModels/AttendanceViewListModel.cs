namespace FarmOps.Models.AttendanceModels
{
    public class AttendanceViewListModel
    {
        public long RosterID { get; set; }
        public string? SignInTime { get; set; }
        public string? SignOutTime { get; set; }
        public decimal? Pay { get; set; }
        public bool OnPaidLeave { get; set; }
        public bool JobNotPaid { get; set; }
    }
    public class AttendanceViewListModel2
    {
        public long RosterID { get; set; }
        public TimeOnly? SignInTime { get; set; }
        public TimeOnly? SignOutTime { get; set; }
        public decimal? Pay { get; set; }
        public bool OnPaidLeave { get; set; }
        public bool JobNotPaid { get; set; }
    }
}
