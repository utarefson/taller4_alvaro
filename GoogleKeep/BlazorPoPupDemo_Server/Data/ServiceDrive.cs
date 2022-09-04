using BlazorPoPupDemo_Server.Models;
using BlazorPoPupDemo_Server.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BlazorPoPupDemo_Server.Data
{
    public class ServiceDrive
    {
        private static string[] Scopes = { DriveService.Scope.DriveMetadata,DriveService.Scope.DriveReadonly, DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        private static string ApplicationName = "Demo App";
        private static string db = "db_proyect";
        private DriveService service;
        public ServiceDrive()
        {
            try
            {
                UserCredential credential = null;
                using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.ReadWrite))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                }
                service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
            }
            catch
            {
                service = null; 
            }
        }
        public Google.Apis.Drive.v3.Data.File GetDB() 
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 800;
            listRequest.Fields = "nextPageToken, files(id, name)";
            Google.Apis.Drive.v3.Data.File file = null;
            try
            {
                IList<Google.Apis.Drive.v3.Data.File> files_temps = listRequest.ExecuteAsync().Result.Files;
                foreach (var item in files_temps)
                {
                    if (item.Name.Equals(db + ".json"))
                    {
                        file = item;
                        break;
                    }
                }
            }
            catch
            {
                file = null;
            }
            return file;
        }
        public Models.Data ListFiles()
        {
            try
            {
                Google.Apis.Drive.v3.Data.File item = GetDB();
                if (item == null) 
                {
                    return new Models.Data {Colors = new List<Color>(),FileId="",Notes=new List<Note>() };
                }
                return GetFile(item.Id); ;
            }
            catch
            {
                return null;
            }
        }
        public DataResponse UpdateList(Models.Data data) 
        {
            try
            {
                if (!data.FileId.Equals("")) 
                {
                    service.Files.Delete(data.FileId).Execute();
                }
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = db+".json"
                };
                FilesResource.CreateMediaUpload request;
                string dataArray = ConvertDataModelAndJson.ConvertObjectToJson(data);
                byte[] messageByte = Encoding.ASCII.GetBytes(dataArray);
                MemoryStream streama = new MemoryStream(messageByte);

                request = service.Files.Create(
                        fileMetadata, streama, "application/json");
                
                request.Fields = "*";
                request.Upload();
                while (request.ResponseBody == null) ;
                var file = request.ResponseBody;

                return new DataResponse { StatusCod = 200, Message = "Saved" };
            }
            catch (Exception e) 
            {
                return new DataResponse { StatusCod = 300, Message = e.Message };
            }
        }
        public Models.Data GetFile(string id) 
        {
            try
            {
                var request = service.Files.Get(id);
                var stream = new MemoryStream();
                bool processingComplete = false;
                request.MediaDownloader.ProgressChanged +=
                    (IDownloadProgress progress) =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    processingComplete = true;
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                var re = request.DownloadAsync(stream);
                try
                {
                    while (!processingComplete) ;
                    var result = Encoding.ASCII.GetString(stream.ToArray());
                    Models.Data data = ConvertDataModelAndJson.ConvertJsonToObject(result);
                    data.FileId = id;
                    int index = 0;
                    foreach (var item in data.Notes) 
                    {
                        item.Id = index;
                        index++;
                    }
                    return data;
                }
                catch(Exception e)
                {
                    return null;
                } 
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
