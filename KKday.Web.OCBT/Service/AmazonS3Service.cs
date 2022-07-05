using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using KKday.Web.OCBT.AppCode;

namespace KKday.Web.OCBT.Service
{

    public class AwsS3ResultModel
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
    }

    public class AwsS3DownloadResultModel : AwsS3ResultModel
    {
        public string ContentType { get; set; }
        public byte[] DataBytes { get; set; }
    }

    ////////////////////////////////////////

    /*

    AWS 區域中命名 S3 存儲桶(Bucket)的規則：
        • 存儲桶名稱在 Amazon S3 中的所有現有存儲桶名稱中必須唯一。
        • 存儲桶名稱必須符合 DNS 命名約定。
        • 存儲桶名稱的長度必須為至少 3 個字符，且不能超過 63 個字符。
        • 存儲桶名稱不能包含大寫字符或下劃線。
        • 存儲桶名稱必須以小寫字母或數字開頭。
        • 存儲桶名稱必須是一系列的一個或多個標籤。相鄰標籤通過單個句點 (.) 分隔。存儲桶名稱可以包含小寫字母、數字和連字符。每個標籤都必須以小寫字母或數字開頭和結尾。
        • 存儲桶名稱不得採用 IP 地址格式（例如，192.168.5.4）。
        • 當通過安全套接字 (SSL) 使用虛擬託管式存儲桶時，SSL 通配符證書僅匹配不包含句點的存儲桶。要解決此問題，請使用 HTTP 或編寫自己的證書驗證邏輯。在使用虛擬託管式存儲桶時，建議您不要在存儲桶名稱中使用句點（“.”）。

    */

    public class AmazonS3Service
    {
        private static string accessKey = string.Empty;
        private static string accessSecret = string.Empty;
        private static string bucket = string.Empty;
        private static string regionEP = string.Empty;
        private static IAmazonS3 s3Client = null;

        public AmazonS3Service()
        {
            accessKey = Website.Instance.AWSaccessKey;
            accessSecret = Website.Instance.AWSaccessSecret;
            bucket = Website.Instance.AWSbucket;
            regionEP = Website.Instance.AWSregionEP;

            // 連到 Amazon S3 主機
            var aws_ep = Amazon.RegionEndpoint.GetBySystemName(regionEP);
            s3Client = new AmazonS3Client(accessKey, accessSecret, aws_ep);

        }

        /*
        #region BucketConfiguration

        public LifecycleConfiguration GetBucketConfiguration()
        {
            // Retrieve current configuration
            var configuration = s3Client.GetLifecycleConfiguration(new GetLifecycleConfigurationRequest
            {
                BucketName = bucket
            }).Configuration;

            return configuration;
        }

        public void SaveBucketConfiguration(LifecycleConfiguration configuration)
        {
            // Save the updated configuration
            s3Client.PutLifecycleConfiguration(new PutLifecycleConfigurationRequest
            {
                BucketName = bucket,
                Configuration = configuration
            });
        }

        public void RemoveBucketConfiguration()
        {
            s3Client.DeleteLifecycleConfiguration(new DeleteLifecycleConfigurationRequest
            {
                BucketName = bucket
            });
        }

        public void SetBucketConfigurationWithExpiration(int expiryDays)
        {
            var rule1 = new LifecycleRule
            {
                Transitions = new List<LifecycleTransition>()
                {
                    new LifecycleTransition
                    {
                        Days = 60,
                        StorageClass = S3StorageClass.Standard
                    }
                },
                Expiration = new LifecycleRuleExpiration
                {
                    Days = expiryDays
                },
                Status = LifecycleRuleStatus.Enabled
            };

            // Save the updated configuration
            s3Client.PutLifecycleConfiguration(new PutLifecycleConfigurationRequest
            {
                BucketName = bucket,
                Configuration = new LifecycleConfiguration
                {
                    Rules = new List<LifecycleRule>() { rule1 }
                }

            });
        }

        #endregion BucketConfiguration
         */

        public async Task<AwsS3ResultModel> UploadObject(string fileName, string contentType, byte[] fileBytes)
        {
            try
            {
                // create unique file name for prevent the mess
                var keyName = fileName;

                PutObjectResponse response = null;

                using (var stream = new MemoryStream(fileBytes))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = bucket,
                        Key = keyName,
                        InputStream = stream,
                        ContentType = contentType
                        //,CannedACL = S3CannedACL.PublicRead
                        // FilePath = filePath
                    };

                    //request.Metadata.Add("param1", "Value1");
                    //request.Metadata.Add("param2", "Value2");

                    response = await s3Client.PutObjectAsync(request);
                };

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // this model is up to you, in my case I have to use it following;
                    return new AwsS3ResultModel
                    {
                        Success = true,
                        FileName = keyName
                    };
                }
                else
                {
                    // this model is up to you, in my case I have to use it following;
                    return new AwsS3ResultModel
                    {
                        Success = false,
                        FileName = keyName
                    };
                }

            }
            catch (Exception ex)
            {
                //_log4netHelper.Info($"{ex.Message}");
                throw ex;
            }

        }

        public async Task<AwsS3DownloadResultModel> GetObject(String keyName)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = keyName
                };

                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (MemoryStream ms = new MemoryStream())
                {
                    responseStream.CopyTo(ms);

                    //var meta1 = response.Metadata["param1"];
                    //var meta2 = response.Metadata["param2"];

                    var result = new AwsS3DownloadResultModel()
                    {
                        Success = true,
                        FileName = keyName,
                        ContentType = response.Headers["Content-Type"],
                        DataBytes = ms.ToArray()
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                //_log4netHelper.Info($"{ex.Message}");
                throw ex;
            }

        }

        public async Task<AwsS3ResultModel> RemoveObject(String fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = fileName
            };

            var response = await s3Client.DeleteObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new AwsS3ResultModel
                {
                    Success = true,
                    FileName = fileName
                };
            }
            else
            {
                return new AwsS3ResultModel
                {
                    Success = false,
                    FileName = fileName
                };
            }
        }

    }
}
