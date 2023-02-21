using System.Security.Claims;
using API.Controllers;
using Entity;
using Entity.DTOs;
using Entity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace API.Hubs
{
    [Authorize]
    public class UserTaskHub : Hub
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFamilyRepository _familyRepository;

        private readonly ITaskRepository _userTaskRepository;
        public UserTaskHub(IFamilyRepository familyRepository,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        ITaskRepository userTaskRepository
    )
        {
            _httpContextAccessor = httpContextAccessor;
            _familyRepository = familyRepository;
            _userManager = userManager;
            _userTaskRepository = userTaskRepository;
        }

        public override async Task OnConnectedAsync()
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, await getGroupName());

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var httpContext = Context.GetHttpContext();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, await getGroupName());
        }


        private async Task<string> getGroupName()
        {
            var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext.User.GetUsername());
            var familyGuid = await _familyRepository.getFamilyGuid(user.Id);
            var family = await _familyRepository.getFamilyMembers(familyGuid, user.Id);
            var familyGroupName = user.FamilyId;
            return familyGroupName.ToString();
        }

        public async Task<Task> TriggerGroup(string userTask)
        {
            var resultUserTaskDto = JsonConvert.DeserializeObject<UserTask>(userTask);

            var user = await _userManager.FindByEmailAsync(_httpContextAccessor.HttpContext.User.GetUsername());
            var familyGuid = await _familyRepository.getFamilyGuid(user.Id);
            await _userTaskRepository.AddTaskAsync(resultUserTaskDto);

            var allMyTasks = await _userTaskRepository.GetAllTasksAysnc(familyGuid);
            return Clients.Group(await getGroupName()).SendAsync("AddTodo", JsonConvert.SerializeObject(allMyTasks));
        }
        
    }
}