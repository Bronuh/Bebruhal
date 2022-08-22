using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.Attachments;

namespace Vk_Module
{
	internal static class Utils
	{
		private static VkApi API;

		public static void SetApi(VkApi api)
		{
			API = api;
		}

		public static MediaAttachment PhotoFromUrl(string url)
		{
			Console.WriteLine("Getting photo from " + url);
			var wc = new WebClient();
			wc.DownloadFile(url, "temp.png");
			var uploadServer = API.Photo.GetMessagesUploadServer(VkModule.config.GroupId);
			var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, "temp.png"));
			var photo = API.Photo.SaveMessagesPhoto(result);//.SaveMessagesPhoto(url);
			wc.Dispose();

			return photo.FirstOrDefault();
		}

		public static MediaAttachment PhotoFromUrlJpg(string url)
		{
			Console.WriteLine("Getting photo from " + url);
			var wc = new WebClient();
			wc.DownloadFile(url, "temp.jpg");
			var uploadServer = API.Photo.GetMessagesUploadServer(VkModule.config.GroupId);
			var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, "temp.jpg"));
			var photo = API.Photo.SaveMessagesPhoto(result);//.SaveMessagesPhoto(url);
			wc.Dispose();
			return photo.FirstOrDefault();
		}

		public static MediaAttachment Photo(string path)
		{
			Console.WriteLine("Getting photo from " + path);
			var uploadServer = API.Photo.GetMessagesUploadServer(VkModule.config.GroupId);
			var wc = new WebClient();
			var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
			var photo = API.Photo.SaveMessagesPhoto(result);
			wc.Dispose();
			return photo.FirstOrDefault();
		}

		public static MediaAttachment Document(string path)
		{
			Console.WriteLine("Getting document from " + path);
			var uploadServer = API.Docs.GetMessagesUploadServer(VkModule.config.GroupId);
			var wc = new WebClient();
			var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
			var doc = API.Docs.Save(result, Path.GetFileNameWithoutExtension(path), tags: null);
			wc.Dispose();
			List<VkNet.Model.Attachments.MediaAttachment> _attachments = new List<VkNet.Model.Attachments.MediaAttachment>();
			foreach (var a in doc)
				_attachments.Add(a.Instance);
			return _attachments.FirstOrDefault();
		}

		public static MediaAttachment ImageDocument(Image image, long peerId)
		{
			Console.WriteLine("Getting document from image");
			var uploadServer = API.Docs.GetMessagesUploadServer(VkModule.config.GroupId);
			Directory.CreateDirectory("vkImages");
			var path = Path.Combine("vkImages", $"tmpImg-{new Random().Next(0,Int32.MaxValue)}.png");
			image.Save(path);
			var wc = new WebClient();
			var result = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
			var doc = API.Docs.Save(result, Path.GetFileNameWithoutExtension(path), tags: null);
			wc.Dispose();
			List<VkNet.Model.Attachments.MediaAttachment> _attachments = new List<VkNet.Model.Attachments.MediaAttachment>();
			var files = Directory.GetFiles("vkImages");
			foreach (var file in files)
			{
				try
				{
					File.Delete(file);
				}
				catch (Exception e)
				{

				}
			}
			foreach (var a in doc)
				_attachments.Add(a.Instance);
			return _attachments.FirstOrDefault();
		}
	}
}
