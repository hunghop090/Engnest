using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace Engnest.Entities.Common
{
	public static class AmazonS3Uploader
	{

		private const string accessKey = "AKIAJZMY3NCCEJQXACJA";
		private const string secretKey = "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";
		private const string bucketname = "engnestasia";


		private static AmazonS3Client client = new AmazonS3Client(
				accessKey,
				secretKey,
				Amazon.RegionEndpoint.APSoutheast1
				);

		public static string UploadFile(string base64String, string type, int width = 0)
		{
			var stringbase64 = "";
			var typeFile = "";
			if (type == TypeUpload.IMAGE)
			{
				typeFile = ".jpg";
				var data64 = Regex.Split(base64String, ";base64,");
				if (data64.Length > 1)
					stringbase64 = data64[1];
				else
					stringbase64 = data64[0];
				//base64String = ImageToBase64(ResizeByWidth(stringbase64, width));
			}
			string Key = type + "/" + CommonFunction.GetTimestamp(DateTime.UtcNow) + "_" + CommonFunction.RandomNumber(0, 99999999) + typeFile;
			try
			{
				//var data = Regex.Split(base64String, ";base64,");
				//if (data.Length > 1)
				//	data[0] = data[1];
				byte[] bytes = Optiame(stringbase64);
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

		public static Image ResizeByWidth(string Base64, int width)
		{
			Image img = Base64ToImage(Base64);

			// lấy chiều rộng và chiều cao ban đầu của ảnh
			int originalW = img.Width;
			int originalH = img.Height;
			if (width == 0 && img.Width > 1100)
				width = 1100;
			if (img.Width < width)
				width = img.Width;
			// lấy chiều rộng và chiều cao mới tương ứng với chiều rộng truyền vào của ảnh (nó sẽ giúp ảnh của chúng ta sau khi resize vần giứ được độ cân đối của tấm ảnh
			int resizedW = width;
			int resizedH = (originalH * resizedW) / originalW;

			// tạo một Bitmap có kích thước tương ứng với chiều rộng và chiều cao mới
			Bitmap bmp = new Bitmap(resizedW, resizedH);

			// tạo mới một đối tượng từ Bitmap
			Graphics graphic = Graphics.FromImage((Image)bmp);
			graphic.InterpolationMode = InterpolationMode.Bicubic;

			// vẽ lại ảnh với kích thước mới
			graphic.DrawImage(img, 0, 0, resizedW, resizedH);

			// gải phóng resource cho đối tượng graphic
			graphic.Dispose();

			// trả về anh sau khi đã resize
			return (Image)bmp;
		}

		public static Image Base64ToImage(string base64String)
		{
			// Convert base 64 string to byte[]
			byte[] imageBytes = Convert.FromBase64String(base64String);
			// Convert byte[] to Image
			using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
			{
				Image image = Image.FromStream(ms, true);
				return image;
			}
		}

		public static string ImageToBase64(Image img)
		{
			using (MemoryStream m = new MemoryStream())
			{
				img.Save(m, ImageFormat.Bmp);
				byte[] imageBytes = m.ToArray();

				// Convert byte[] to Base64 String
				string base64String = Convert.ToBase64String(imageBytes);
				return base64String;
			}
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
		public static string GetUrl(string key, int Expires = 1)
		{
			try
			{
				GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
				request.BucketName = bucketname;
				request.Key = key;
				if (Expires != 0)
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
		public static byte[] Optiame(string image)
		{

			var ImageInfo = Base64ToImage(image);
			Size size = new Size(0, 0);
			var changesize = false;
			byte[] photoBytes = Convert.FromBase64String(image);
			if (ImageInfo.Width > 800)
			{
				size.Width = 800;
				using (MemoryStream inStream = new MemoryStream(photoBytes))
				{
					using (MemoryStream outStream = new MemoryStream())
					{
						// Initialize the ImageFactory using the overload to preserve EXIF metadata.
						using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
						{
							// Load, resize, set the format and quality and save an image.
							imageFactory.Load(inStream)
										.Resize(size)
										.Save(outStream);
						}
						photoBytes = outStream.ToArray();
						image = Convert.ToBase64String(photoBytes);
						changesize = true;
					}
				}
			}
			var n = image.Length;
			var sizeInBytes = 4 * Math.Ceiling((decimal)(n / 3)) * (decimal)0.5624896334383812;
			var QualityDecimal = (100000 / sizeInBytes) * 100;
			var Quality = Decimal.ToInt32(Math.Round((100000 / sizeInBytes) * 100, 0, MidpointRounding.AwayFromZero));
			if (Quality == 0)
				Quality = 1;
			else if (Quality > 100)
				Quality = 100;
			else if (Quality > 50 && Quality < 100 && !changesize)
				Quality = Quality / 2;

			// Format is automatically detected though can be changed.
			ISupportedImageFormat format = new JpegFormat { Quality = Quality };
			using (MemoryStream inStream = new MemoryStream(photoBytes))
			{
				using (MemoryStream outStream = new MemoryStream())
				{
					// Initialize the ImageFactory using the overload to preserve EXIF metadata.
					using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
					{
						// Load, resize, set the format and quality and save an image.
						imageFactory.Load(inStream)
									.Format(format)
									.Save(outStream);
					}
					return outStream.ToArray();
				}
			}
		}
	}
}