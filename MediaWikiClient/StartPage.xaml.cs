using MediaWikiClient.Factories;
using MediaWikiClient.Services;

namespace MediaWikiClient;

public partial class StartPage
{
    private readonly Constants _constants = new();
    private readonly IDataService _dataService = new DataService();
    private readonly IMediaWikiApi _mediaWikiApi = new MediaWikiApi();

    public StartPage()
    {
        InitializeComponent();
        AdresseDbEntry.Placeholder = _constants.DbAdresse;
        UsernameDbEntry.Placeholder = _constants.DbUsername;
        PasswordDbEntry.Placeholder = _constants.DbPassword;
        DbEntry.Placeholder = _constants.DbName;
        CertAutoSwitch.IsToggled = _constants.TrustServerCertificate;
    }

    private void SwitchDb_OnToggled(object sender, ToggledEventArgs e)
    {
        AdresseDbEntry.IsEnabled = !AdresseDbEntry.IsEnabled;
        UsernameDbEntry.IsEnabled = !UsernameDbEntry.IsEnabled;
        PasswordDbEntry.IsEnabled = !PasswordDbEntry.IsEnabled;
        DbEntry.IsEnabled = !DbEntry.IsEnabled;
        CertAutoSwitch.IsEnabled = !CertAutoSwitch.IsEnabled;

        SaveBtn.Text = e.Value ? "Enregistrer" : "Terminer";
    }

    private void SwitchApi_OnToggled(object sender, ToggledEventArgs e)
    {
        AdresseApiEntry.IsEnabled = !AdresseApiEntry.IsEnabled;

        SaveBtn.Text = e.Value ? "Enregistrer" : "Terminer";
    }

    private async void SaveBtn_OnClicked(object sender, EventArgs e)
    {
        ActivityIndicatorTerminer.IsEnabled = true;
        ActivityIndicatorTerminer.IsRunning = true;

        if (SwitchDb.IsToggled)
        {
            if (AdresseDbEntry.Text != null)
                Preferences.Set("dbAdresse", AdresseDbEntry.Text);
            if (UsernameDbEntry.Text != null)
                Preferences.Set("dbUsername", UsernameDbEntry.Text);
            if (PasswordDbEntry.Text != null)
                Preferences.Set("dbPassword", PasswordDbEntry.Text);
            if (DbEntry.Text != null)
                Preferences.Set("dbName", DbEntry.Text);
            if (AdresseApiEntry.Text != null)
                Preferences.Set("endpointApi", AdresseApiEntry.Text);
            if (CertAutoSwitch.IsToggled)
                Preferences.Set("trustServerCertificate", CertAutoSwitch.IsToggled);

            var dbnewString = _dataService.NewConnectionString();
            var dbresult = await _dataService.TestConnection();
            if (!dbresult && !dbnewString)
            {
                await DisplayAlert("Erreur", "Impossible de se connecter à la base de données", "OK");
                AdresseDbEntry.Focus();
            }
        }

        var apiresult = await _mediaWikiApi.TestConnection();
        if (!apiresult)
        {
            await DisplayAlert("Erreur", "Impossible de se connecter à l'API", "OK");
            AdresseApiEntry.Focus();
        }

        ActivityIndicatorTerminer.IsEnabled = false;
        ActivityIndicatorTerminer.IsRunning = false;
        if (apiresult)
        {
            Preferences.Set("isconfigured", true);
            Preferences.Set("isdbconfigured", SwitchDb.IsToggled);
            Application.Current!.MainPage = new AppShell();
        }
    }
}