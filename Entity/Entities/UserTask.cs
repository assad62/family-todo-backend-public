using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.Entities;

namespace Entity
{
    public class UserTask
    {
        public Guid Id { get; set; }

        public string TaskName { get; set; }

        public string Title {get;set;} 

       

        [ForeignKey(nameof(FamilyId))]
        public Guid FamilyId { get; set; }

        public Family Family { get; set; }

        public string Image { get; set; }

        public UserTaskStatus TaskStatus {get;set;} = UserTaskStatus.InProgress;

        public string CreatedBy {get; set;}

        public DateTime createdDate {get;set;} = DateTime.UtcNow;
    }
}