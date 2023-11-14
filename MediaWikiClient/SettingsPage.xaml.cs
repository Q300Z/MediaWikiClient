using MediaWikiClient.Factories;
using MediaWikiClient.Services;

namespace MediaWikiClient;

public partial class SettingsPage
{
    private readonly Constants _constants;
    private readonly IDataService _dataService;
    private readonly IMediaWikiApi _mediaWikiApi;

    public SettingsPage()
    {
        InitializeComponent();
        _dataService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IDataService>();
        _mediaWikiApi = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IMediaWikiApi>();
        _constants = Application.Current.MainPage.Handler.MauiContext.Services.GetService<Constants>();
        BindingContext = _constants;

        AdresseDbEntry.Text = _constants.DbAdresse;
        UsernameDbEntry.Text = _constants.DbUsername;
        PasswordDbEntry.Text = _constants.DbPassword;
        DbEntry.Text = _constants.DbName;
        CertAutoSwitch.IsToggled = _constants.TrustServerCertificate;
        AdresseApiEntry.Text = _constants.EndpointApi;
        SwitchDb.IsToggled = _constants.IsDbConfigured;
    }

    private async void SaveBtn_OnClicked(object sender, EventArgs e)
    {
        ActivityIndicatorDropSave.IsEnabled = true;
        ActivityIndicatorDropSave.IsRunning = true;

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

            var a = Preferences.Get("dbPassword", "");

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

        ActivityIndicatorDropSave.IsEnabled = false;
        ActivityIndicatorDropSave.IsRunning = false;
        if (apiresult)
        {
            Preferences.Set("isconfigured", true);
            await DisplayAlert("Succès", "Connexion réussi !", "OK");
        }
    }

    private void SwitchDb_OnToggled(object sender, ToggledEventArgs e)
    {
        AdresseDbEntry.IsEnabled = !AdresseDbEntry.IsEnabled;
        UsernameDbEntry.IsEnabled = !UsernameDbEntry.IsEnabled;
        PasswordDbEntry.IsEnabled = !PasswordDbEntry.IsEnabled;
        DbEntry.IsEnabled = !DbEntry.IsEnabled;
        CertAutoSwitch.IsEnabled = !CertAutoSwitch.IsEnabled;
    }

    private async void DeleteBtn_OnClicked(object sender, EventArgs e)
    {
        // Merci copilot pour l'allemand xD
        var result = await DisplayAlert("Warnung", "Sind Sie sicher, dass Sie die Datenbank löschen wollen?", "Ja", "Nein");
        if (result)
        {
            ActivityIndicatorDropDb.IsEnabled = true;
            ActivityIndicatorDropDb.IsRunning = true;
            Preferences.Clear();
            Application.Current!.MainPage = new StartPage();
            if (await _dataService.DropDatabase()) await DisplayAlert("Erfolg", "Datenbank erfolgreich gelöscht", "OK");

            ActivityIndicatorDropDb.IsEnabled = false;
            ActivityIndicatorDropDb.IsRunning = false;
        }
    }
}