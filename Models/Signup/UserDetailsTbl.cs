namespace FarmOps.Models.Signup
{
    public class UserDetailsTbl
    {
        public string UserId { get; set; } = "W00001";// Unique identifier for the user
            public string? FirstName { get; set; } // First name of the user
            public string? LastName { get; set; } // Last name of the user
            public string? MiddleName { get; set; } // Middle name of the user
            public string? Picture { get; set; } // Picture URL or path
            public DateTime? Dob { get; set; } // Date of birth, nullable
            public string? PassportNumber { get; set; } // Passport number
            public string? Phone { get; set; } // Phone number
            public int? PayRate { get; set; } // Pay rate of the user, nullable
            public string? Ird { get; set; } // IRD number
            public bool? ForkliftCert { get; set; } // Forklift certification, nullable
            public string? Ir330 { get; set; } // IR330 form code or tax code
            public string? Licence { get; set; } // Licence number
            public bool? WorkAuth { get; set; } // Work authorization, nullable
            public string? WorkType { get; set; } // Type of work or job role
            public int? WorkEvId { get; set; } // Work event ID, nullable
            public string? Document { get; set; } // Document related to the user
            public string? Ir330Document { get; set; } // IR330 document
            public string? AccNum { get; set; } // Account number
            public string? PayrollId { get; set; } // Payroll ID
            public string? PreEmployment { get; set; } // Pre-employment documents
            public string? SignPic { get; set; } // Signature picture
    }
}
