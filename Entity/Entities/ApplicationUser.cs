using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [ForeignKey(nameof(FamilyId))]
        public Guid FamilyId { get; set; }
        public Family Family { get; set; }

        public virtual IList<UserEvent> EventsList { get; set; }
        public virtual IList<UserTask> TaskList { get; set; }

        public string DeviceId {get;set;}
    }
}