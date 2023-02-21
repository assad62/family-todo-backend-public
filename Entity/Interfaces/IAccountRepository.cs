using Entity.DTOs;
using Entity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Entity.Interfaces
{
    public interface IAccountRepository
    {
        
        Task<RegisterUserDto> RegisterUserAsync(RegisterUser registerUser);
        Task<LoginUserDto> LoginUserAsync(LoginUser loginUser);
    }
}