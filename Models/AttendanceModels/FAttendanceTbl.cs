using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class FAttendanceTbl
{
    [Key]
    [Column(Order = 1)]
    public long RosterId { get; set; } // Roster ID (BIGINT)

    [Key]
    [Column(Order = 2)]
    public DateTime AttendanceDate { get; set; } // Attendance Date (DATE)

    [Key]
    [Column(Order = 3)]
    public TimeSpan StartTime { get; set; } // Start Time (TIME(7))

    [Key]
    [Column(Order = 4)]
    public TimeSpan EndTime { get; set; } // End Time (TIME(7))

    public decimal? TotalHours { get; set; } // Total work hours (DECIMAL(4, 2))
    public decimal? BreakTime { get; set; }  // Break time (DECIMAL(4, 2))
    public int? BlockId { get; set; } // Block ID (INT)
    public decimal? Pay { get; set; } // Pay (DECIMAL(6, 2))

    [StringLength(10)]
    public string AttendanceType { get; set; } // Attendance Type (NCHAR(10))

    public int? JobId { get; set; } // Job ID (INT)
    public int? PaidBreak { get; set; } // Paid Break (INT)

    [StringLength(200)]
    public string? AttendanceSignPic { get; set; } // Attendance Sign Picture (NVARCHAR(200))

    public int? LineId { get; set; } // Line ID (INT)
    public string? JobPaid { get; set; } // Job Paid (BINARY(1))

    [StringLength(500)]
    public string? Remarks { get; set; } // Remarks (NVARCHAR)

    [StringLength(100)]
    public string? AppliedBy { get; set; } // Applied By (NVARCHAR)

    public string ApprovedStatus { get; set; } // Approved Status (INT)

    [StringLength(100)]
    public string? ApprovedBy { get; set; } // Approved By (NVARCHAR)

    public DateTime? ApprovedDt { get; set; } // Approved Date (DATETIME)
}
