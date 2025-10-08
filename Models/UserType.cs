using System.ComponentModel.DataAnnotations;

namespace Readify_Library.Models
{
    public enum enTypesNames : byte
    {
        Normal = 1,
        Premium = 2,
        VIP = 3
    }

    public class UserType
    {
        public int Id { get; set; }
        public enTypesNames TypeName { get; set; }

        [Range(0, 50, ErrorMessage = "Extra Books Must be Betwwen 0 - 50")]
        public int ExtraBooks { get; set; }

        [Range(0, 100, ErrorMessage = "Extra Days Must be Betwwen 0 - 100")]
        public int ExtraDays { get; set; }

        [Range(0, 1000, ErrorMessage = "Extra Penalty Must be Betwwen 0 - 1000")]
        public decimal ExtraPenalty { get; set; }


        public List<ApplicationUser>? Users { get; set; }
    }
}
