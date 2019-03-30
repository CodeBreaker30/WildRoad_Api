using System.ComponentModel.DataAnnotations;

namespace WildRoadApp_Api.API.DataContracts
{
    public class User
    {
        [DataType(DataType.Text)]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Firstname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Lastname { get; set; }

        public Adress Adress { get; set; }

    }
}
