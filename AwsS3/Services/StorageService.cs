using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AwsS3.Models;
using Amazon.S3;
using Amazon.S3.Transfer;
using AwsS3.Config;
using Amazon.Runtime;

namespace AwsS3.Services
{
    public class StorageService : IStorageService
{
    

    public async Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues)
    {
        var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);

        var config = new AmazonS3Config() 
        {
            RegionEndpoint = Amazon.RegionEndpoint.APNortheast2
        };

        var response = new S3ResponseDto();
        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = obj.InputStream,
                Key = obj.Name,
                BucketName = obj.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            // initialise client
            using var client = new AmazonS3Client(credentials, config);

            // initialise the transfer/upload tools
            var transferUtility = new TransferUtility(client);

            // initiate the file upload
            await transferUtility.UploadAsync(uploadRequest);

            response.StatusCode = 201;
            var itemUrl = $"https://work-tasks-storage-bucket.s3.{Amazon.RegionEndpoint.APNortheast2.OriginalSystemName}.amazonaws.com/{obj.Name}";
            response.Url = itemUrl; 
            response.Message = "Upload Success";
        }
        catch(AmazonS3Exception s3Ex)
        {
            response.StatusCode = (int)s3Ex.StatusCode;
            response.Message = s3Ex.Message;
        }
        catch(Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
        }

        return response;
    }
}
}