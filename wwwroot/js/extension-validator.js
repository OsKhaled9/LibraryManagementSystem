// Cover Client Side Validation
$.validator.addMethod("extension", function (value, element, parameter) {
    if (this.optional(element)) return true;
    var extension = value.split('.').pop().toLowerCase();
    return $.inArray(extension, parameter.split(',')) !== -1;
});