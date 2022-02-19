namespace ASPNetUserService.Domain.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

}
