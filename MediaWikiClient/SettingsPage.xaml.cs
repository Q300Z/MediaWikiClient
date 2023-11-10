using MediaWikiClient.Factories;

namespace MediaWikiClient;

public partial class SettingsPage
{
    private readonly IDataService _dataService;

    public SettingsPage()
    {
        InitializeComponent();
        _dataService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IDataService>();
        var constants = Application.Current.MainPage.Handler.MauiContext.Services.GetService<Constants>();
        BindingContext = constants;

        AdresseDbEntry.Text = constants.DbAdresse;
        UsernameDbEntry.Text = constants.DbUsername;
        PasswordDbEntry.Text = constants.DbPassword;
        DbEntry.Text = constants.DbName;
        CertAutoSwitch.IsToggled = constants.TrustServerCertificate;
        AdresseApiEntry.Text = constants.EndpointApi;
    }

    private async void SaveBtn_OnClicked(object sender, EventArgs e)
    {
        ActivityIndicatorDropSave.IsEnabled = true;
        ActivityIndicatorDropSave.IsRunning = true;
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

        var dbresult = await _dataService.TestConnection();
        if (!dbresult)
        {
            await DisplayAlert("Erreur", "Impossible de se connecter à la base de données", "OK");
            AdresseDbEntry.Focus();
        }

        ActivityIndicatorDropSave.IsEnabled = false;
        ActivityIndicatorDropSave.IsRunning = false;
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
            Application.Current!.MainPage = new ConfigPage();
            if (await _dataService.DropDatabase())
            {
                await DisplayAlert("Erfolg", "Datenbank erfolgreich gelöscht", "OK");
                
            }

            ActivityIndicatorDropDb.IsEnabled = false;
            ActivityIndicatorDropDb.IsRunning = false;
        }
    }
}