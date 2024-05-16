namespace Clinic.Models
{
    public class Mail
    {
        public List<string> EmailTo { get; set; } = new List<string>();
        public string EmailFromId { get; set; } = "lazare.kaladze@yahoo.com";
        public string EmailFromPassword { get; set; } = "Ok";
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public List<string>? Attachments { get; set; } = new List<string>();
    }
}
