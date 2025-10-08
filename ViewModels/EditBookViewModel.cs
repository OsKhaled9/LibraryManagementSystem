using Readify_Library.Attributes;
using Readify_Library.Settings;

namespace Readify_Library.ViewModels
{
    public class EditBookViewModel : BookFormViewModel
    {
        public int Id { get; set; }

        public string? CurrentCover { get; set; }

        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile? BookCover { get; set; } = default!;
    }
}
