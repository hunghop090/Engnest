using System;
using Amazon.S3;
using Amazon.S3.Model;

namespace Engnest.Entities.Common
{
	public static class AmazonS3Uploader
	{
			
		private const string accessKey = "AKIAJZMY3NCCEJQXACJA";
		private const string secretKey = "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";
		private const string bucketname = "engnest";

		private static AmazonS3Client client = new AmazonS3Client(
				accessKey,
				secretKey,
				Amazon.RegionEndpoint.USEast2
				);
		public static string UploadFile(string filePath, string type)
		{
			string Key = type + "/" + CommonFunction.GetTimestamp(DateTime.Now) + "_" + CommonFunction.RandomNumber(0, 99999999);
			try
			{
				ListBucketsResponse response = client.ListBuckets();
				foreach (S3Bucket b in response.Buckets)
				{
					Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
				}
				PutObjectRequest request = new PutObjectRequest();
				request.BucketName = bucketname;
				request.FilePath = filePath;
				request.Key = Key;
				request.CannedACL = S3CannedACL.PublicRead;
				var a = client.PutObject(request);
			}
			catch (AmazonS3Exception amazonS3Exception)
			{
				if (amazonS3Exception.ErrorCode != null &&
					(amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
					||
					amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
				{
					throw new Exception("Check the provided AWS Credentials.");
				}
				else
				{
					throw new Exception("Error occurred: " + amazonS3Exception.Message);
				}
			}

			return Key;
		}

		public static string GetUrl(string key)
		{
			GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
			request.BucketName = bucketname;
			request.Key = key;
			request.Expires = DateTime.Now.AddHours(1);
			request.Protocol = Protocol.HTTP;
			string url = client.GetPreSignedURL(request);
			return url;
		}
	}
}