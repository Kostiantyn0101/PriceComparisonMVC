namespace PriceComparisonMVC.Models.User
{
    public class UserProfileModel
    {
        public string Username { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? ReviewsCount { get; set; }
       
    }
}
