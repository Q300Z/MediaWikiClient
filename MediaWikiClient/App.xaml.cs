namespace MediaWikiClient;

public partial class App : Application
{
    private readonly Constants _constants = new();

    public App()
    {
        InitializeComponent();
        if (_constants.IsConfigured)
            MainPage = new AppShell();
        else
            MainPage = new ConfigPage();
    }
}