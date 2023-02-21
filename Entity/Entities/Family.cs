using System.Text.Json.Serialization;

namespace Entity
{
    public class Family
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public string Image { get; set; }

        public string FamilyIdentifier { get; set; } = Guid.NewGuid().ToString("n").Substring(0, 6);

        public virtual ICollection<ApplicationUser> ListOfUsers { get; set; }
    }
}