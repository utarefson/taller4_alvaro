using System.Collections.Generic;

namespace BlazorPoPupDemo_Server.Models
{
    public class Data
    {
        public Data()
        {
            Notes = new List<Note>();
            Colors = new List<Color>();
        }
        public string FileId { get; set; }
        public List<Note> Notes { get; set; }
        public List<Color>Colors{ get; set; }
    }
}
