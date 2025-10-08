using Readify_Library.Attributes;
using Readify_Library.Settings;

namespace Readify_Library.ViewModels
{
    public class CreateBookViewModel : BookFormViewModel
    {
        [AllowedExtensions(FileSettings.AllowedExtensions),
        MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile BookCover { get; set; } = default!;
    }
}
