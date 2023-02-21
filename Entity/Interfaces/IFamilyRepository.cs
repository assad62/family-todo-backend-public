using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace Entity.Interfaces
{
    public interface IFamilyRepository
    {
        Task<FamilyMembersDto> getFamilyMembers(Guid familyUID, string userId);

        public Task<Guid> getFamilyGuid(string userID);

        public Task<FamilyIdentifierDto> getFamilyIdentifier(string userID);

        public Task updateFamilyField(Guid familyId, JsonPatchDocument familyFieldToUpdate);


        public Task<FamilyNameDto> getFamilyName(Guid familyId);

        public Task<FamilyProfileDto> getFamilyProfile(Guid familyId, string userId);
        public IEnumerable<ApplicationUser> getListOfFamilyUsers(Guid familyId, String userID);


    }
}