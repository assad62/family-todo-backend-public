using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AwsS3.Models
{
    public class S3ResponseDto
    {
        
      public int StatusCode { get; set; }
      public string? Url { get; set; }
      public string? Message { get; set; }

    }
}