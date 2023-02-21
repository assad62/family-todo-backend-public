using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class FamilyProfileDto
    {
        public string name { get; set; }
        public string Image { get; set; }

        public string familyIdentifier { get; set; }

        public virtual List<String> ListOfFamilyMembers { get; set; }
    }
}