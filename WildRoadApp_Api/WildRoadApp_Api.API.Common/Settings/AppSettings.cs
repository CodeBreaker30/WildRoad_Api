using System.ComponentModel.DataAnnotations;

namespace WildRoadApp_Api.API.Common.Settings
{
    public class AppSettings
    {
        [Required]
        public API API { get; set; }
        [Required]
        public Swagger Swagger { get; set; }
    }

    public class API
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }

    public class Swagger
    {
        [Required]
        public bool Enabled { get; set; }
    }
}
