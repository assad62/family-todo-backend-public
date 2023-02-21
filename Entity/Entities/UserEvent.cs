using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class UserEvent
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }
        
        [ForeignKey(nameof(ApplicationUserId))]
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}