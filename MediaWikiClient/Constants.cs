namespace MediaWikiClient;

public class Constants
{
    public bool IsConfigured => Preferences.Get("isconfigured", false);

    public string DbAdresse => Preferences.Get("dbAdresse", "localhost");
    public string DbUsername => Preferences.Get("dbUsername", "sa");
    public string DbPassword => Preferences.Get("dbPassword", "yourpassword");
    public string DbName => Preferences.Get("dbName", "mediawiki");
    public bool TrustServerCertificate => Preferences.Get("trustServerCertificate", true);

    public string EndpointApi => Preferences.Get("endpointApi", "https://fr.wikipedia.org/w/api.php");
}