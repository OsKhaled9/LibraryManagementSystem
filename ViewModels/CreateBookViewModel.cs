using Readify_Library.Attributes;
using Readify_Library.Settings;
using System.ComponentModel.DataAnnotations;

namespace Readify_Library.ViewModels
{
    public class CreateBookViewModel : BookFormViewModel
    {
        [Display(Name = "Book Cover")]
        [AllowedExtensions(FileSettings.AllowedExtensions),
        MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile BookCover { get; set; } = default!;
    }
}
