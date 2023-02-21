using Entity;
using Entity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {

        private readonly IFamilyRepository _familyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationHub(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IFamilyRepository familyRepository)
        {
            _familyRepository = familyRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

        }
        private async Task<string> getGroupName()
        {
            var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext.User.GetUsername());
            var familyGuid = await _familyRepository.getFamilyGuid(user.Id);
            var family = await _familyRepository.getFamilyMembers(familyGuid, user.Id);
            var familyGroupName = user.FamilyId;
            return familyGroupName.ToString();
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("In OnConnected");
            await Groups.AddToGroupAsync(Context.ConnectionId, await getGroupName());

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("In OnDisConnected");
            var httpContext = Context.GetHttpContext();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, await getGroupName());
        }


    }
}
