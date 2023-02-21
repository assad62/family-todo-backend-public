using Entity.DTOs;
using Microsoft.AspNetCore.JsonPatch;

namespace Entity.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<UserTask>>GetAllTasksAysnc(Guid familyId);

        Task<UserTask> GetTaskByIdAsync(Guid taskId);
        
        Task<AddTaskDto> AddTaskAsync(UserTask userTask);

        Task DeleteTaskAsync(Guid taskId);

        Task UpdateTaskAsync(UserTask userTask);

        Task UpdateTaskPatchAsync(Guid taskId, JsonPatchDocument userTask);

        Task<List<UserTask>>GetInProgressTasksAsync(Guid familyId);
        Task<List<UserTaskDTO>>GetDoneTasksAsync();
        Task<List<UserTaskDTO>>GetExpiredTasksAsync();

        
    }
}