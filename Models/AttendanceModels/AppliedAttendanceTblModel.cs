using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmOps.Models.AttendanceModels
{
    public class AppliedAttendanceTblModel
    {
        [Key]  // Marking this as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  // Ensuring RosterID is not auto-generated.
        [Column("attendanceId")]
        public long? AttendanceId { get; set; } // Nullable bigint

        [Column("RosterID")]
        public long RosterID { get; set; } // Non-nullable bigint

        [Column("signinTime")]
        public TimeOnly? SigninTime { get; set; } // Nullable datetime

        [Column("signoutTime")]
        public TimeOnly? SignoutTime { get; set; } // Nullable datetime

        [Column("totalWorkHours")]
        [DataType(DataType.Currency)]
        public decimal? TotalWorkHours { get; set; } // Nullable decimal (29, 17)

        [Column("totalBreakHours")]
        [DataType(DataType.Currency)]
        public decimal? TotalBreakHours { get; set; } // Nullable decimal (29, 17)

        [Column("breakIds")]
        [StringLength(int.MaxValue)] // Used for nvarchar(max)
        public string BreakIds { get; set; } // Nullable nvarchar(max)

        [Column("pay")]
        [Range(0, 999999.99)]
        public decimal Pay { get; set; } // Non-nullable decimal (6, 2)

        [Column("job_cat")]
        public int JobCat { get; set; } // Non-nullable int

        [Column("on_paid_leave")]
        public bool OnPaidLeave { get; set; } // Non-nullable bit

        [Column("lineid")]
        public int LineId { get; set; } // Non-nullable int

        [Column("jobnotpaid")]
        public bool JobNotPaid { get; set; } // Non-nullable bit

        [Column("InsertId")]
        [StringLength(20)] // nvarchar(20)
        public string InsertId { get; set; } // Non-nullable nvarchar(20)

        [Column("InsertDt")]
        public DateTime InsertDt { get; set; } // Non-nullable datetime

        [Column("UpdtId")]
        [StringLength(20)] // nvarchar(20)
        public string UpdtId { get; set; } // Nullable nvarchar(20)

        [Column("UpdtDt")]
        public DateTime? UpdtDt { get; set; } // Nullable datetime

        [Column("attendDate")]
        public DateTime AttendanceDate { get; set; } // Non-nullable date
    }
}