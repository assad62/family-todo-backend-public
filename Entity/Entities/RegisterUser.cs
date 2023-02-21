using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class RegisterUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string FamilyIdentifier {get; set;}

        public string DeviceId {get;set;}

        
    }
}