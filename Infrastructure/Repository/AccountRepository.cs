using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity;
using Entity.DTOs;
using Entity.Entities;
using Entity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AccountRepository : IAccountRepository
    {

        private readonly AppDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;


        public AccountRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDataContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<LoginUserDto> LoginUserAsync(LoginUser loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, false);

            if (!result.Succeeded)
            {
                return null;
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
            );



            var response = new LoginUserDto()
            {
                Email = loginUser.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token)

            };

            return response;
        }



        public async Task<RegisterUserDto> RegisterUserAsync(RegisterUser registerUser)
        {

            var familyRecords = await _context.Family.ToListAsync();

            Family family = null;
            try
            {
                family = familyRecords.FirstOrDefault(c => c.FamilyIdentifier == registerUser.FamilyIdentifier);

            }
            catch (Exception e)
            {
                Console.WriteLine("Ex " + e);
            }


            var user = new ApplicationUser()
            {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email,
                UserName = registerUser.Email,
                FamilyId = family != null ? family.Id : Guid.NewGuid(),
                Family = family != null ? family : new Family(),
                DeviceId = registerUser.DeviceId
                
            };


            var createUser = await _userManager.CreateAsync(user, registerUser.Password);

            if (!createUser.Succeeded)
                return null;

            var registerResponse = new RegisterUserDto()
            {
                Email = user.Email,
                FamilyIdentifier = user.Family.FamilyIdentifier
            };




            return registerResponse;
        }
    }
}