using MediaWikiClient.Factories;
using MediaWikiClient.Models;

namespace MediaWikiClient;

public partial class HistoryPage : ContentPage
{
    private readonly IDataService _dataService;
    public HistoryPage()
    {
        InitializeComponent();
        _dataService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IDataService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetHistory();
    }

    private async void SetHistory()
    {
        try
        {
            var articles = await _dataService.SearchArticle();
            if (articles != null && articles.Count > 0)
            {
                var articleGroups = new List<ArticleGroup>
                {
                    new(true, articles.Where(e => e.IsFavoris).OrderBy(e => e.DateLu).ToList()),
                    new(false, articles.Where(e => !e.IsFavoris && e.IsLu).OrderBy(e => e.DateLu).ToList())
                };
                HistoryList.ItemsSource = articleGroups;
            }
        }
        catch (Exception e)
        {
            await DisplayAlert("Erreur", "Impossible de charger l'historique : \n " + e.Message, "OK");
        }
    }

    private async void HistoryList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Article article) return;
        await Navigation.PushAsync(new DetailsArticlePage(article), true);
    }

    private async void ClearHistory_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Confirmation", "Voulez-vous vraiment supprimer l'historique ?", "Oui", "Non");
        if (result)
            if (await _dataService.ClearHistory())
            {
                SetHistory();
                await DisplayAlert("Succès", "Historique supprimé avec succès", "OK");
            }
    }
}