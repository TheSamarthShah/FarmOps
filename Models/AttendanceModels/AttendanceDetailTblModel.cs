using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmOps.Models.AttendanceModels
{
    public class AttendanceDetailTblModel
    {
        [Key]  // Specifies that this is the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Ensures the ID is not auto-generated
        [Column("attendanceId")]
        public long AttendanceId { get; set; } // Non-nullable bigint

        [Column("approvedStatus")]
        [Required]
        [StringLength(1)]
        public string ApprovedStatus { get; set; } // Non-nullable char(1)

        [Column("approvedBy")]
        [StringLength(20)] // nvarchar(20)
        public string ApprovedBy { get; set; } // Nullable nvarchar(20)

        [Column("attendanceSheetPhoto")]
        [StringLength(255)] // nvarchar(255)
        public string AttendanceSheetPhoto { get; set; } // Nullable nvarchar(255)

        [Column("rejectedRosterIds")]
        public string RejectedRosterIds { get; set; } // Nullable nvarchar(max)

        [Column("approvedDt")]
        public DateTime? ApprovedDt { get; set; } // Nullable datetime

        [Column("appliedBy")]
        [StringLength(20)] // nvarchar(20)
        public string AppliedBy { get; set; } // Nullable nvarchar(20)

        [Column("remarks")]
        [StringLength(255)] // nvarchar(255)
        public string Remarks { get; set; } // Nullable nvarchar(255)

        [Column("attendanceDate")]
        [Required]
        public DateTime AttendanceDate { get; set; } // Non-nullable date
    }
}