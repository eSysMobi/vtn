using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Net;
using System.IO;

namespace Vlastelin.Common
{
	public static class FileManager
	{
		private const long BUFFER_SIZE = 4096;

		/// <summary>
		/// добавление файла в архив
		/// </summary>
		/// <param name="zipFilename">имя архива</param>
		/// <param name="fileToAdd">имя добавляемого файла</param>
		public static void AddFileToZip(string zipFilename, string fileToAdd)
		{
			using (Package zip = Package.Open(zipFilename, FileMode.OpenOrCreate))
			{
				string destFilename = ".\\" + Path.GetFileName(fileToAdd);
				Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
				if (zip.PartExists(uri))
				{
					zip.DeletePart(uri);
				}
				PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
				using (FileStream fileStream = GetFileStream(fileToAdd, FileAccess.Read, 1, 5))
				{
					using (Stream dest = part.GetStream())
					{
						CopyStream(fileStream, dest);
					}
				}
			}
		}

		private static void CopyStream(System.IO.FileStream inputStream, System.IO.Stream outputStream)
		{
			long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
			byte[] buffer = new byte[bufferSize];
			int bytesRead = 0;
			long bytesWritten = 0;
			while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				outputStream.Write(buffer, 0, bytesRead);
				bytesWritten += bufferSize;
			}
		}
		
		/// <summary>
		/// Возвращает файловый поток
		/// </summary>
		/// <param name="file"></param>
		/// <param name="currentTryCount">Количество попыток для открытия файла</param>
		/// <returns></returns>
		public static FileStream GetFileStream(string file, FileAccess access, int currentTryCount, int maximumTries)
		{
			String IOFileAccessError = "The process cannot access the file";
			try
			{
				return new FileStream(file, FileMode.Open, access);
			}
			catch (IOException ex)
			{
				if (ex.Message.StartsWith(IOFileAccessError) && currentTryCount <= maximumTries)
				{
					System.Threading.Thread.Sleep(200);
					return GetFileStream(file, access, ++currentTryCount, maximumTries);
				}
				else
					throw;
			}
		}
		
		/// <summary>
		/// загрузает файл на фтп
		/// </summary>
		/// <param name="destFileName">имя файла на фтп</param>
		/// <param name="UploadFileName">имя загружаемого файла</param>
		/// <param name="ftpServerIP">фтп сервер</param>
		/// <param name="ftpUserID">логин пользователя</param>
		/// <param name="ftpPassword">пароль пользователя</param>
		/// <param name="enableSSL">вкллючить SSL</param>
		public static void FTPUpload(string destFileName, String UploadFileName, string ftpServerIP, string ftpUserID, string ftpPassword)
		{
			FileStream inputStream = GetFileStream(UploadFileName, FileAccess.Read, 1, 200);

			// создаем вебреквест для загрузки файла
			FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(String.Concat("ftp://", ftpServerIP, "/report/", destFileName)));

			// вводим креденшалы
			reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
			reqFTP.KeepAlive = false;

			// метод загрузки
			reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
			reqFTP.Timeout = 20000;

			// тип передачи файла
			reqFTP.UsePassive = false;
			reqFTP.UseBinary = true;
			reqFTP.KeepAlive = false;

			// размер файла
			reqFTP.ContentLength = inputStream.Length;

			// буфер
			int buffLength = 2048;
			byte[] buff = new byte[buffLength];
			int contentLen;
			//Stream strm = null;
			Stream reqStream = null;  

			try
			{
				// выгрузка файла на фтп
				reqStream = reqFTP.GetRequestStream();
				contentLen = inputStream.Read(buff, 0, buffLength);
				while (contentLen != 0)
				{
					reqStream.Write(buff, 0, contentLen);
					contentLen = inputStream.Read(buff, 0, buffLength);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (reqStream != null) reqStream.Close();
				if (inputStream != null) inputStream.Close();
			}
		}
	}
}
