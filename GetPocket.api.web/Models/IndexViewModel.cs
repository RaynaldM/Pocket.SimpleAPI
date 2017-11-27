using System.ComponentModel.DataAnnotations;

namespace GetPocket.api.web.Models
{
    public class IndexViewModel
    {
        [Required]
        [Display(Name = "GetPocket platform consumer key")]
        [DataType(DataType.Text)]
        public string ConsumerKey { get; set; }
    }
}