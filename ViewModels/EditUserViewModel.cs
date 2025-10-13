using Readify_Library.Attributes;
using Readify_Library.Settings;

namespace Readify_Library.ViewModels
{
    public class EditUserViewModel : UserFormViewModel
    {
        public string? CurrenProfileImage { get; set; }

        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile? ProfileImage { get; set; } = default!;
    }
}
