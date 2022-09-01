using BlazorPoPupDemo_Server.Models;
using BlazorPoPupDemo_Server.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorPoPupDemo_Server.Data
{
    public class ServiceDrive
    {
        public static string[] Scopes = { DriveService.Scope.DriveMetadata,DriveService.Scope.DriveReadonly, DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        public static string ApplicationName = "Demo App";
        public static string db = "db_proyect";
        private DriveService service;
        public ServiceDrive() //FileAccess.ReadWrite,
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

            try
            {
                IList<Google.Apis.Drive.v3.Data.File> files_temps = listRequest.ExecuteAsync().Result.Files;
                foreach (var item in files_temps)
                {
                    if (item.Name.Equals(db+".json"))
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Models.Data ListFiles()
        {
            
            try
            {
                Google.Apis.Drive.v3.Data.File item = GetDB();
                
                return GetFile(item.Id); ;
            }
            catch
            {
                return null;
            }
        }
        public DataResponse CreateFile(string name,string type) 
        {
            try
            {
                Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File();
                file.Name = name;
                file.MimeType = type;
                var request = service.Files.Create(file);
                var response = request.Execute();
                return new DataResponse { StatusCod = 200, Message = "File Created" };
            }
            catch (Exception e) 
            {
                return new DataResponse { StatusCod = 300, Message = e.Message };
            }
            //@onclick="@(()=>modal.Show<Counter>(" SUBSCRIBE"))"

        }
        public DataResponse UpdateList(Models.Data data) 
        {
            try
            {
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
                request.ProgressChanged += Request_ProgressChanged1;
                while (request.ResponseBody == null) ;
                var file = request.ResponseBody;

                return new DataResponse { StatusCod = 200, Message ="Guardado" };
            }
            catch (Exception e) 
            {
                return new DataResponse { StatusCod = 300, Message = e.Message };
            }
        }
        private void Request_ProgressChanged1(Google.Apis.Upload.IUploadProgress obj)
        {
            switch (obj.Status)
            {
                case UploadStatus.Uploading:
                    {
                        Console.Write(obj.BytesSent.ToString());
                    }
                    break;
            }
        }
        public Models.Data GetFile(string id) 
        {
            try
            {
                var request = service.Files.Get(id);
                var stream = new System.IO.MemoryStream();
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
                /*Task.Delay(10000).ContinueWith((task) => {
                    var result = Encoding.ASCII.GetString(stream.ToArray());
                    return result;
                });*/
                try
                {
                    while (!processingComplete) ;
                    var result = Encoding.ASCII.GetString(stream.ToArray());
                    
                    Models.Data data = ConvertDataModelAndJson.ConvertJsonToObject(result);
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
