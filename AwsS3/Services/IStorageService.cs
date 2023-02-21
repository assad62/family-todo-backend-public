using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsS3.Models;

namespace AwsS3.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues);

        //TODO:- delete file
    }
}