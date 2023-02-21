using API.Helpers;
using API.Hubs;
using Entity;
using Entity.DTOs;
using Entity.Entities;
using Entity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PushNotification.Models;
using PushNotification.Services;

namespace API.Controllers
{
    [Authorize]
    public class TaskController : UserController
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskRepository _userTaskRepository;

        private readonly IFamilyRepository _familyRepository;

        private readonly INotificationService _notificationService;

        public Task<ApplicationUser> CurrentUser
        {
            get
            {
                var loggedInUser = getLoggedInUser();
                var applicationUser = _userManager.FindByIdAsync(loggedInUser.Result.Id);
                return applicationUser;
            }
        }

        public TaskController(ITaskRepository userTaskRepository,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService,
            IFamilyRepository familyRepository

            ) : base(userManager)
        {
            _userTaskRepository = userTaskRepository;
            _userManager = userManager;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _familyRepository = familyRepository;
        }


        private async Task<Guid> getLoggedInUserFamilyId()
        {
            var loggedInUser = await getLoggedInUser();
            return loggedInUser.FamilyId;
        }

        private async Task<string> getGroupName()
        {
            var familyId = await getLoggedInUserFamilyId();
            return familyId.ToString();
        }


        [HttpPost("")]
        public async Task<IActionResult> CreateTask([FromBody] UserTask userTask)
        {
            WebResponse<AddTaskDto> response = new WebResponse<AddTaskDto>();
            try
            {
                if (userTask == null)
                {
                    response.Message = "UserTask object is required.";
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }

                var loggedInUser = await getLoggedInUser();

                var user = await _userManager.FindByIdAsync(loggedInUser.Id);
                userTask.FamilyId = user.FamilyId;
                userTask.CreatedBy = user.FirstName + " " + user.LastName;
                userTask.TaskStatus = UserTaskStatus.InProgress;



                response.Data = await _userTaskRepository.AddTaskAsync(userTask);
                response.Message = string.Format("{0} created task {1} successfully", user.FirstName, userTask.TaskName);

                var listOfFamilyMembers = _familyRepository.getListOfFamilyUsers(user.FamilyId, loggedInUser.Id);
                var listOfDeviceIds = listOfFamilyMembers.Select(e => e.DeviceId).ToList();
                

                //TODO: use push notification groups, instead of looping over ids like this
                foreach (var deviceId in listOfDeviceIds)
                {
                    
                    var notificationModel = new NotificationModel();
                    notificationModel.DeviceId = deviceId;
                    notificationModel.IsAndroiodDevice = true;
                    notificationModel.Body = "";
                    notificationModel.Title = string.Format("{0} created task {1} successfully", user.FirstName, userTask.TaskName);
                    await _notificationService.SendNotification(notificationModel);
                }

                if (response.Data == null)
                    return StatusCode(StatusCodes.Status401Unauthorized);




                await _hubContext.Clients.Group(await getGroupName()).SendAsync("TaskUpdate", string.Format("{0} created task {1} successfully", user.FirstName, userTask.TaskName));

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("familytasks")]
        public async Task<IActionResult> GetLoggedinUserFamilyTasks()
        {
            WebResponse<List<UserTask>> response = new WebResponse<List<UserTask>>();
            try
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Data = await _userTaskRepository.GetAllTasksAysnc(await getLoggedInUserFamilyId());
                response.Message = "Retrieved data successfully";
                if (response.Data == null || response.Data.Count == 0)
                {
                    response.Message = "Please enter a correct familyId.";
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet("familytasks/in_progress")]
        public async Task<IActionResult> GetLoggedinUserInProgressFamilyTasks()
        {
            var loggedInUser = await getLoggedInUser();
            var user = await _userManager.FindByIdAsync(loggedInUser.Id);

            WebResponse<List<UserTask>> response = new WebResponse<List<UserTask>>();
            try
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Data = await _userTaskRepository.GetInProgressTasksAsync(user.FamilyId);
                response.Message = "Retrieved data successfully";
                if (response.Data == null || response.Data.Count == 0)
                {
                    response.Message = "Please enter a correct familyId.";
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpGet("familytasks/{familyId}")]
        public async Task<IActionResult> GetAllTasksByFamilyIdAysnc(Guid familyId)
        {
            WebResponse<List<UserTask>> response = new WebResponse<List<UserTask>>();
            try
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Data = await _userTaskRepository.GetAllTasksAysnc(familyId);
                response.Message = "Retrieved data successfully";
                if (response.Data == null || response.Data.Count == 0)
                {
                    response.Message = "Please enter a correct familyId.";
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskByIdAsync(Guid taskId)
        {
            WebResponse<UserTask> response = new WebResponse<UserTask>();
            try
            {
                response.Data = await _userTaskRepository.GetTaskByIdAsync(taskId);
                response.Message = "Retrieved data successfully";

                if (response.Data == null)
                {
                    response.Message = "Please enter a correct taskId.";
                    return StatusCode((int)StatusCodes.Status404NotFound, response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTaskAsync(Guid TaskId)
        {
            WebResponse<Task> response = new WebResponse<Task>();
            try
            {
                await _userTaskRepository.DeleteTaskAsync(TaskId);
                response.Message = "Deleted task successfully";

                await _hubContext.Clients.Group(await getGroupName()).SendAsync("TaskUpdate", string.Format("{0} deleted task {1} successfully", CurrentUser.Result.FirstName, TaskId));

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateTask([FromBody] UserTask userTask)
        {
            WebResponse<Task> response = new WebResponse<Task>();
            try
            {
                await _userTaskRepository.UpdateTaskAsync(userTask);
                response.Message = "Updated task successfully";
                await _hubContext.Clients.Group(await getGroupName()).SendAsync("TaskUpdate", string.Format("{0} updated task {1} successfully", CurrentUser.Result.FirstName, userTask.TaskName));
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            return StatusCode(StatusCodes.Status201Created, response);
        }


        [HttpPatch("{taskId}")]
        public async Task<IActionResult> UpdateTaskPatch([FromBody] JsonPatchDocument userTask, [FromRoute] Guid taskId)
        {

            WebResponse<Task> response = new WebResponse<Task>();
            try
            {
                await _userTaskRepository.UpdateTaskPatchAsync(taskId, userTask);
                response.Message = "Patched task successfully";
                response.StatusCode = StatusCodes.Status200OK;
                await _hubContext.Clients.Group(await getGroupName()).SendAsync("TaskUpdate", string.Format("{0} completed a task successfully", CurrentUser.Result.FirstName));


            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status304NotModified;
                response.Message = ex.Message;
                return StatusCode(StatusCodes.Status304NotModified, response);
            }



            return StatusCode(StatusCodes.Status200OK, response);
        }



    }
}