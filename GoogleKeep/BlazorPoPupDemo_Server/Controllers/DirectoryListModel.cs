using System.Collections.Generic;

namespace BlazorPoPupDemo_Server.Controllers
{
    public class DirectoryListModel
    {
        public string DirectoryId { get; set; }

        public string DirectoryName { get; set; }
        public List<Google.Apis.Drive.v3.Data.File> Files { get; set; }
    }
}
