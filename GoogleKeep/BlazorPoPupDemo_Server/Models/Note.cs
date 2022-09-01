namespace BlazorPoPupDemo_Server.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Color Color { get; set; }
        public int Archived { get; set; }//1
        public int Remove { get; set; }//1
    }
}
