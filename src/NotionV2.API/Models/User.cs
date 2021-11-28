using System.ComponentModel.DataAnnotations;

namespace NotionV2.API.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}