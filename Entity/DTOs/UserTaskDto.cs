using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.Entities;

namespace Entity.DTOs
{
     public class UserTaskDTO
    {
       

        public string Image { get; set; }

        public string TaskName {get; set;}

        public string Title {get;set;}

        public string CreatedBy {get;set;}

        public UserTaskStatus TaskStatus {get;set;} = UserTaskStatus.InProgress;
    }
}