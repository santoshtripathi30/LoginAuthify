public class LoginProviderSettings
{
    // List of available authentication providers
    public static readonly List<string> AvailableProviders = new()
    {
        "Google",
        "Facebook",
        "GitHub",
        "Twitter",
        "LinkedIn",
        "Microsoft"
    };

    // Currently enabled providers
    public static HashSet<string> EnabledProviders { get; set; } = new();

    public static bool IsProviderEnabled(string provider) => EnabledProviders.Contains(provider);

    public static void UpdateEnabledProviders(IEnumerable<string> providers)
    {
        EnabledProviders.Clear();
        EnabledProviders.UnionWith(providers);
    }

    public static IEnumerable<string> GetEnabledProviders() => EnabledProviders;
}
