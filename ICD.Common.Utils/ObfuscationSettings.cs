using System.Reflection;

[assembly: Obfuscation(Feature = "platform api: System.Threading.Thread, System.Reflection.*, System.IO.Stream, System.Windows.Forms.*", Exclude = true)]
[assembly: Obfuscation(Feature = "rename symbol names with printable characters", Exclude = false)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
[assembly: Obfuscation(Feature = "Apply to type * when class: renaming", Exclude = true, ApplyToMembers = false)]
