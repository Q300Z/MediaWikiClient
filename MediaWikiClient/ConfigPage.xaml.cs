using MediaWikiClient.Factories;
using MediaWikiClient.Services;

namespace MediaWikiClient;

public partial class ConfigPage : ContentPage
{
    private readonly Constants _constants = new();
    private readonly IDataService _dataService = new DataService();
    private readonly IMediaWikiApi _mediaWikiApi = new MediaWikiApi();

    public ConfigPage()
    {
        InitializeComponent();
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
        if (AdresseApiLabel.Text != null)
            Preferences.Set("endpointApi", AdresseApiEntry.Text);
        
        
        var dbresult = await _dataService.TestConnection();
        if(!dbresult)
        {
            await DisplayAlert("Erreur", "Impossible de se connecter à la base de données", "OK");
            AdresseDbEntry.Focus();
        }
        
        var apiresult = await _mediaWikiApi.TestConnection();
        if(!apiresult)
        {
            await DisplayAlert("Erreur", "Impossible de se connecter à l'API", "OK");
            AdresseApiEntry.Focus();
        }
        
        if (dbresult && apiresult)
        {
            Preferences.Set("isconfigured", true);
            Application.Current.MainPage = new AppShell();
        }
    }
}