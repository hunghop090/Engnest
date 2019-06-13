using System;
using Amazon.S3;
using Amazon.S3.Model;

namespace Engnest.Entities.Common
{
	public static class AmazonS3Uploader
	{
		public static string UploadFile(string filePath)
		{
			string Key = CommonFunction.GetTimestamp(DateTime.Now) + "_" + CommonFunction.RandomNumber(0,99999999);
			string accessKey = "AKIAJZMY3NCCEJQXACJA";
			string secretKey = "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";

			AmazonS3Client client = new AmazonS3Client(
					accessKey,
					secretKey,
					Amazon.RegionEndpoint.USEast2
					);
			try
			{
				ListBucketsResponse response = client.ListBuckets();
				foreach (S3Bucket b in response.Buckets)
				{
					Console.WriteLine("{0}\t{1}", b.BucketName, b.CreationDate);
				}
				PutObjectRequest request = new PutObjectRequest();
				request.BucketName = "engnest";
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
	}
}