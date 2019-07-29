using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using TinifyAPI;

namespace Engnest.Entities.Common
{
	public static class UploadImageTifiny
	{

		private const string accessKey = "AKIAJZMY3NCCEJQXACJA";
		private const string secretKey = "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";
		private const string bucketname = "engnestasia";


		private static AmazonS3Client client = new AmazonS3Client(
				accessKey,
				secretKey,
				Amazon.RegionEndpoint.APSoutheast1
				);

		public static async Task<string> TinifyModulAsync(string base64String, string type)
		{
			try
			{
				var stringbase64 = "";
				var data64 = Regex.Split(base64String, ";base64,");
				if (data64.Length > 1)
					stringbase64 = data64[1];
				else
					stringbase64 = data64[0];
				byte[] bytes = System.Convert.FromBase64String(stringbase64);
				Tinify.Key = "3bfrr65Nl2QYQTk8lGHGQ8w8ZGJ358ND";
				var source = Tinify.FromBuffer(bytes);
				string Key = type + "/" + CommonFunction.GetTimestamp(DateTime.UtcNow) + "_" + CommonFunction.RandomNumber(0, 99999999);
				await source.Store(new
				{
					service = "s3",
					aws_access_key_id = "AKIAJZMY3NCCEJQXACJA",
					aws_secret_access_key = "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",
					region = "ap-southeast-1",
					path = bucketname+"/"+Key,
				});
				return Key;
			}
			catch (AccountException e)
			{
				return "";
				// Verify your API key and account limit.
			}
			catch (ClientException e)
			{
				return "";
				// Check your source image and request options.
			}
			catch (ServerException e)
			{
				return "";
				// Temporary issue with the Tinify API.
			}
			catch (ConnectionException e)
			{
				return "";
				// A network connection error occurred.
			}
			catch (System.Exception e)
			{
				return "";
				// Something else went wrong, unrelated to the Tinify API.
			}
		}
	}
}