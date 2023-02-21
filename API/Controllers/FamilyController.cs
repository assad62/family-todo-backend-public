using API.Helpers;
using AwsS3;
using AwsS3.Models;
using AwsS3.Services;
using Entity;
using Entity.DTOs;
using Entity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FamilyController : UserController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFamilyRepository _familyRepository;
        private readonly IStorageService _storageService;

        private readonly IConfiguration _config;
        public FamilyController(IFamilyRepository familyRepository, UserManager<ApplicationUser> userManager,
        IStorageService storageService, IConfiguration configuration
        ) : base(userManager)
        {
            _familyRepository = familyRepository;
            _userManager = userManager;
            _storageService = storageService;
            _config = configuration;
        }


        [HttpGet("members")]
        public async Task<IActionResult> getFamilyMembers()
        {
            WebResponse<FamilyMembersDto> response = new WebResponse<FamilyMembersDto>();
            var loggedInUser = await getLoggedInUser();

            try
            {
                var result = await _familyRepository.getFamilyMembers(loggedInUser.FamilyId, loggedInUser.Id);
                response.Message = "Success";
                response.StatusCode = 200;
                response.Data = result;
            }
            catch (Exception e)
            {
                response.Message = "Unauthorized " + e.Message;
                response.StatusCode = 401;
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }


            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpGet("family_identifier")]
        public async Task<IActionResult> getFamilyIdentifier()
        {

            WebResponse<FamilyIdentifierDto> response = new WebResponse<FamilyIdentifierDto>();

            var loggedInUser = await getLoggedInUser();

            var result = await _familyRepository.getFamilyIdentifier(loggedInUser.Id);


            if (result == null)
            {
                response.Message = "No result found";
                response.StatusCode = 401;
                return StatusCode(StatusCodes.Status401Unauthorized, response);

            }

            response.Data = result;
            response.Message = "Success";
            response.StatusCode = 200;
            return StatusCode(StatusCodes.Status200OK, response);


        }


        [HttpPatch("image")]
        public async Task<IActionResult> updateFamilyImage([FromBody] JsonPatchDocument newName)
        {
            var loggedInUser = await getLoggedInUser();
            WebResponse<Task> response = new WebResponse<Task>();
            try
            {

                await _familyRepository.updateFamilyField(loggedInUser.FamilyId, newName);
                response.Message = "Patched family field successfully";
                response.StatusCode = 200;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, e.Message);
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPatch("name")]
        public async Task<IActionResult> updateFamilyName([FromBody] JsonPatchDocument newName)
        {
            var loggedInUser = await getLoggedInUser();
            WebResponse<Task> response = new WebResponse<Task>();
            try
            {

                await _familyRepository.updateFamilyField(loggedInUser.FamilyId, newName);
                response.Message = "Patched family field successfully";
                response.StatusCode = 200;
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, e.Message);
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }


        [HttpGet("name")]
        public async Task<IActionResult> getFamilyName()
        {
            WebResponse<FamilyNameDto> response = new WebResponse<FamilyNameDto>();
            var loggedInUser = await getLoggedInUser();

            try
            {
                var familyName = await _familyRepository.getFamilyName(loggedInUser.FamilyId);
                response.StatusCode = 200;
                response.Data = familyName;
                response.Message = "Success";

                return StatusCode(StatusCodes.Status200OK, response);

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }


        [HttpGet("family_profile")]
        public async Task<IActionResult> getFamilyProfile()
        {
          
            WebResponse<FamilyProfileDto> response = new WebResponse<FamilyProfileDto>();
            var loggedInUser = await getLoggedInUser();


            try
            {
                var result = await _familyRepository.getFamilyProfile(loggedInUser.FamilyId, loggedInUser.Id);
                response.Message = "Success";
                response.Data = result;

            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Data = null;

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }


            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPost("image/upload")]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {

            // Process file
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.FileName);
            var docName = $"{file.FileName}.{Guid.NewGuid()}{fileExt}";
            // call server

            var s3Obj = new S3Object()
            {
                BucketName = "work-tasks-storage-bucket",
                InputStream = memoryStream,
                Name = docName
            };

            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"]
            };

            var result = await _storageService.UploadFileAsync(s3Obj, cred);
            WebResponse<FileUploadDto> response = new WebResponse<FileUploadDto>();
            response.Message = result.Message;
            response.StatusCode = 201;

            var fileUploadDto = new FileUploadDto()
            {
                url = result.Url
            };
            response.Data = fileUploadDto;


            return StatusCode(StatusCodes.Status201Created, response);

        }




    }
}