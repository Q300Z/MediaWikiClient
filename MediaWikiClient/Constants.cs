namespace MediaWikiClient;

public class Constants
{
    public bool IsConfigured { get;  } = Preferences.Get("isconfigured", false);

    public string DbAdresse { get; } = Preferences.Get("dbAdresse", "192.168.1.103");
    public string DbUsername { get; } = Preferences.Get("dbUsername", "sa");
    public string DbPassword { get;  } = Preferences.Get("dbPassword", "password@123");
    public string DbName { get; } = Preferences.Get("dbName", "mediawiki");
    public bool TrustServerCertificate { get;  } = Preferences.Get("trustServerCertificate", true);

    public string EndpointApi { get; } = Preferences.Get("endpointApi", "https://fr.wikipedia.org/w/api.php");
}