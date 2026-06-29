namespace PaintShopIMS.Models
{
    /// <summary>
    /// Represents an application user for authentication.
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        
        public bool IsAdmin => Role == "Admin";
    }
}
