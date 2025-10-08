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

        [MinLength(1)]
        [MaxLength(50)]
        public int ExtraBooks { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        public int ExtraDays { get; set; }

        [MinLength(1)]
        [MaxLength(1000)]
        public decimal ExtraPenalty { get; set; }


        public List<ApplicationUser> Users { get; set; }
    }
}
