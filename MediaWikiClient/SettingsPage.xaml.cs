using MediaWikiClient.Factories;

namespace MediaWikiClient;

public partial class SettingsPage : ContentPage
{
    private readonly IDataService _dataService;
    private readonly Constants _constants;
    public SettingsPage()
    {
        InitializeComponent();
        _dataService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IDataService>();
        _constants = Application.Current.MainPage.Handler.MauiContext.Services.GetService<Constants>();
        BindingContext = _constants;
        AdresseDbEntry.Text = _constants.DbAdresse;
        UsernameDbEntry.Text = _constants.DbUsername;
        PasswordDbEntry.Text = _constants.DbPassword;
        DbEntry.Text = _constants.DbName;
        CertAutoSwitch.IsToggled = _constants.TrustServerCertificate;
        AdresseApiEntry.Text = _constants.EndpointApi;
        
        
    }
    private void SaveBtn_OnClicked(object sender, EventArgs e)
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

        Preferences.Set("isconfigured", true);
    }

    private async void DeleteBtn_OnClicked(object sender, EventArgs e)
    {
        // Merci copilot pour l'allemand xD
        var result = await DisplayAlert("Warnung", "Sind Sie sicher, dass Sie die Datenbank löschen wollen?", "Ja", "Nein"); 
        if (result)
        {
            
            if(await _dataService.DropDatabase())
                await DisplayAlert("Erfolg", "Datenbank erfolgreich gelöscht", "OK");
                Preferences.Clear();
                Application.Current.MainPage = new ConfigPage();
        }
    }
}