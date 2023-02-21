using Entity;
using Entity.DTOs;
using Entity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace Infrastructure.Repository
{
    [Authorize]
    public class FamilyRepository : IFamilyRepository
    {
        private readonly AppDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FamilyRepository(AppDataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Guid> getFamilyGuid(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            return user.FamilyId;

        }

        public async Task<FamilyIdentifierDto> getFamilyIdentifier(string userID)
        {




            try
            {
                var user = await _userManager.FindByIdAsync(userID);
                var familyId = user.FamilyId;

                var selectedFamily = _context.Family.ToList().Where(c => c.Id == familyId).First();

                var familyIdentifier = new FamilyIdentifierDto()
                {
                    familyIdentifier = selectedFamily.FamilyIdentifier
                };
                return familyIdentifier;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



            return null;
        }

        public async Task<FamilyMembersDto> getFamilyMembers(Guid familyId, string userId)
        {

            //get family members without current logged in user

            var familyMembers = collateFamilyMembers(familyId,userId);

            var fmd = new FamilyMembersDto()
            {
                FamilyMembers = familyMembers
            };
            return fmd;

        }

        public async Task<FamilyNameDto> getFamilyName(Guid familyId)
        {
            var family = await _context.Family.FindAsync(familyId);
            var familyNameDto = new FamilyNameDto()
            {
                familyName = family.name
            };
            return familyNameDto;
        }

        public async Task<FamilyProfileDto> getFamilyProfile(Guid familyId,string userId)
        {
            var family = await _context.Family.FindAsync(familyId);
           
            var familyProfileDto = new FamilyProfileDto()
            {
                name = family.name,
                familyIdentifier = family.FamilyIdentifier,
                Image = family.Image,
                ListOfFamilyMembers = collateFamilyMembers(familyId,userId)
            };

            return familyProfileDto;
        }

        public async Task updateFamilyField(Guid familyId, JsonPatchDocument familyFieldToUpdate)
        {

            var familyToUpdate = await _context.Family.FindAsync(familyId);

            try
            {
                familyFieldToUpdate.ApplyTo(familyToUpdate);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private List<string> collateFamilyMembers(Guid familyId, String userID)
        {
            // var loggedInUser = await loggedInUser
            var listOfFamilyMembers = _context.Family
                        .Include(b => b.ListOfUsers)
                        .ToList()
                        .Find(x => x.Id == familyId).ListOfUsers
                        .Where(e => e.Id != userID);

            List<string> familyMembers = new List<string>();

            foreach (var i in listOfFamilyMembers)
            {
                familyMembers.Add(i.FirstName + " " + i.LastName);
            }
            return familyMembers;
        }

        public IEnumerable<ApplicationUser> getListOfFamilyUsers(Guid familyId, String userID){
            return _context.Family
                        .Include(b => b.ListOfUsers)
                        .ToList()
                        .Find(x => x.Id == familyId).ListOfUsers
                        .Where(e => e.Id != userID);
                        
        }


    }
}