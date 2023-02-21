using Entity;
using Entity.DTOs;
using Entity.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entity.Entities;

namespace Infrastructure.Repository
{
    public class UserTaskRepository : ITaskRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly AppDataContext _context;
        private readonly IMapper _mapper;
        public UserTaskRepository(AppDataContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<AddTaskDto> AddTaskAsync(UserTask userTask)
        {
            await _context.UserTasks.AddAsync(userTask);
            await _context.SaveChangesAsync();
            var addTaskDto = new AddTaskDto(){
                taskId = userTask.Id.ToString()
            };
            return addTaskDto;
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            _context.UserTasks.Remove(await _context.UserTasks.SingleOrDefaultAsync(x => x.Id == taskId));
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserTask>> GetAllTasksAysnc(Guid familyId)
        {
            var records = await _context.UserTasks.ToListAsync();
            var filteredRecords = records.Where(c => c.FamilyId == familyId).OrderByDescending(x => x.createdDate);

            return _mapper.Map<List<UserTask>>(filteredRecords);

        }

        public Task<List<UserTaskDTO>> GetDoneTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<UserTaskDTO>> GetExpiredTasksAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserTask>> GetInProgressTasksAsync(Guid familyId)
        {
            var records = await _context.UserTasks.ToListAsync();
            var filteredRecords = records
            .Where(c=>c.FamilyId == familyId)
            .Where(c => c.TaskStatus == UserTaskStatus.InProgress)
            .OrderByDescending(x => x.createdDate); ;

            return _mapper.Map<List<UserTask>>(filteredRecords);
        }

        public Task<UserTask> GetTaskByIdAsync(Guid taskId)
        {
            return _context.UserTasks.FirstOrDefaultAsync(x => x.Id == taskId);
        }

        public async Task UpdateTaskAsync(UserTask userTask)
        {
            var task = await _context.UserTasks.FindAsync(userTask.Id);
            if (task != null)
            {
                task.TaskName = userTask.TaskName;
                task.Image = userTask.Image;
                task.TaskStatus = task.TaskStatus;
                task.FamilyId = task.FamilyId;
                await _context.SaveChangesAsync();
            }
        }

        


        public async Task UpdateTaskPatchAsync(Guid taskId, JsonPatchDocument userTask)
        {
            var userTaskToUpdate = await _context.UserTasks.FindAsync(taskId);
            if (userTaskToUpdate != null)
            {
                userTask.ApplyTo(userTaskToUpdate);
                await _context.SaveChangesAsync();
            }
        }
    }
}