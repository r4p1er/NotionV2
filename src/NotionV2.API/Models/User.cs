using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NotionV2.API.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Login { get; set; }
        
        [Required]
        [JsonIgnore]
        public string Password { get; set; }
    }
}