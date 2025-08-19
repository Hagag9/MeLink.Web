
namespace MeLink.Web.Models
{
    public enum Gender { Male , Female }
    public class Patient : ApplicationUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
    }
}
