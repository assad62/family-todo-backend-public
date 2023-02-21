using API.Helpers;
using AwsS3;
using AwsS3.Models;
using AwsS3.Services;
using Entity;
using Entity.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{  
    
    public class UserController : BaseController
    {
         private readonly UserManager<ApplicationUser> _userManager;
    
         public UserController(UserManager<ApplicationUser> userManager) 
        {       
              _userManager = userManager;
            
        }
        public async Task<ApplicationUser>  getLoggedInUser(){
             return await _userManager.FindByEmailAsync(User?.Identity?.Name);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> getProfile(){
             
             WebResponse<ProfileDto> response = new WebResponse<ProfileDto>();
             var loggedInUser = await getLoggedInUser();
             var profile = new ProfileDto(){
                name = loggedInUser.FirstName + " "+ loggedInUser.LastName,
                
             };
             response.StatusCode = 200;
             response.Data = profile;
             response.Message = "Success";
             return StatusCode(StatusCodes.Status200OK, response);
        }

       
        [HttpDelete]
        public async Task deleteAccount()
        {
            var loggedInUser = await getLoggedInUser();
            await _userManager.DeleteAsync(loggedInUser);

            
        }
       

    }
}