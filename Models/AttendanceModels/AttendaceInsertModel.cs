namespace FarmOps.Models.AttendanceModels
{
    public class AttendaceInsertModel
    {

        public List<AttendanceViewListModel>? AttendanceRecords { get; set; } = new List<AttendanceViewListModel>();
        public IFormFile? AttendanceSheetPhoto { get; set; }
        public DateTime AttendanceDate { get; set; }

    }
}
