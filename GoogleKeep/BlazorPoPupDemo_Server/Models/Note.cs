namespace BlazorPoPupDemo_Server.Models
{
    public class Note
    {
        public Note() 
        {
            Color = new Color();
            Id = -1;
            Title = "";
            Content = "";
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Color Color { get; set; }
        public int Archived { get; set; }
        public int Remove { get; set; }
    }
}
