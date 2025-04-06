namespace FarmOps.Models.Signup
{
    public class Login_Insert
    {
        public string Id { get; set; } = "W00001";// Unique identifier for the user
            public string? Email { get; set; } // Email address of the user
            public string? Password { get; set; } // Password for the user
            public string? Type { get; set; } // Type of the user (e.g., "User", "Admin", etc.)

    }
}
