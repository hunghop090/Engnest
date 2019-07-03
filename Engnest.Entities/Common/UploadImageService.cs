using System;
using System.IO;
using System.Text.RegularExpressions;
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
		public static string UploadFile(string base64String, string type)
		{
			string Key = type + "/" + CommonFunction.GetTimestamp(DateTime.UtcNow) + "_" + CommonFunction.RandomNumber(0, 99999999);
			try
			{
				var data = Regex.Split(base64String, ";base64,");
				if (data.Length > 1)
					data[0] = data[1];
				byte[] bytes = Convert.FromBase64String(data[0]);
				PutObjectRequest request = new PutObjectRequest();
				request.BucketName = bucketname;
				request.Key = Key;
				request.CannedACL = S3CannedACL.Private;
				using (var ms = new MemoryStream(bytes))
				{
					request.InputStream = ms;
					client.PutObject(request);
				}
			}
			catch (Exception ex)
			{
				return "";
			}

			return Key;
		}
		public static string UploadFileStream(Stream stream, string type)
		{
			string Key = type + "/" + CommonFunction.GetTimestamp(DateTime.UtcNow) + "_" + CommonFunction.RandomNumber(0, 99999999);
			try
			{
				PutObjectRequest request = new PutObjectRequest();
				request.BucketName = bucketname;
				request.Key = Key;
				request.CannedACL = S3CannedACL.Private;
				request.InputStream = stream;
				client.PutObject(request);
			}
			catch (Exception ex)
			{
				return "";
			}

			return Key;
		}
		public static string GetUrl(string key,int Expires = 1)
		{
			try
			{
				GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
				request.BucketName = bucketname;
				request.Key = key;
				if(Expires != 0)
					request.Expires = DateTime.UtcNow.AddHours(Expires);
				else 
					request.Expires = DateTime.UtcNow.AddDays(7);
				request.Protocol = Protocol.HTTP;
				string url = client.GetPreSignedURL(request);
				return url;
			}
			catch (Exception ex)
			{
				return "";
			}

		}
	}
}