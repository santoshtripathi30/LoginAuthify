namespace LoginAuthify.Models
{
    public class ProviderSettingsVM
    {
        public List<string> AvailableProviders { get; set; } = new();
        public List<string> EnabledProviders { get; set; } = new();
    }
}
