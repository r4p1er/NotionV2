using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NotionV2.API.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required] 
        public string Header { get; set; } = "Untitled";
        
        [Required]
        public string Body { get; set; }
        
        [JsonIgnore]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}