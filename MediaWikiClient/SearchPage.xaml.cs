using System.Diagnostics;
using MediaWikiClient.Factories;
using MediaWikiClient.Models;
using MediaWikiClient.Services;

namespace MediaWikiClient;

public partial class SearchPage : ContentPage
{
    private readonly IDataService _dataService;
    private readonly MediaWikiApi _mediaWikiApi;

    public SearchPage()
    {
        InitializeComponent();
        _dataService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<IDataService>();
        _mediaWikiApi = Application.Current.MainPage.Handler.MauiContext.Services.GetService<MediaWikiApi>();
        BindingContext = this;

        SearchResults.Opacity = 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SearchBar.Focus();
    }

    private async void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchBar = (SearchBar)sender;
        if (searchBar.Text.Trim().Length > 0)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var dbresult = await _dataService.SearchArticle(searchBar.Text.Trim());
                if (dbresult != null && dbresult.Count >= 10)
                    SearchResults.ItemsSource = dbresult;
                else
                    SearchResults.ItemsSource = await GetArticlesFromApi(searchBar.Text.Trim());

                stopwatch.Stop();
                var nbresults = ((List<Article>)SearchResults.ItemsSource)?.Count ?? 0;
                if (nbresults > 0)
                {
                    await NbResults.FadeTo(0.75, 240);
                    NbResults.Text = $"{nbresults} résultats en {stopwatch.ElapsedMilliseconds.ToString()} ms";
                }

                await SearchResults.FadeTo(1);
            }
            catch (Exception exep)
            {
                Console.Error.WriteLine(exep);
                await DisplayAlert("Erreur", $"Une erreur est survenue lors de la recherche : \n {exep}", "OK");
            }
        }
        else
        {
            SearchResults.ItemsSource = null;
            await SearchResults.FadeTo(0);
            NbResults.Text = "Aucun résultat";
            await NbResults.FadeTo(0, 1250);
        }
    }

    private async Task<List<Article>> GetArticlesFromApi(string search)
    {
        SearchProgress.Progress = 0;
        SearchProgress.IsVisible = true;

        var apiresult = await _mediaWikiApi.RechercherArticle(search);

        // Crée une liste de tâches pour ajouter les articles à la base de données de manière asynchrone
        var addTasks = new List<Task<bool>>();


        foreach (var article in apiresult)
        {
            await SearchProgress.ProgressTo((double)apiresult.IndexOf(article) / apiresult.Count, 250, Easing.Linear);
            article.InDatabase = true;
            article.DateInDatabase = DateTime.Now;
            article.InDatabase = !await _dataService.AddArticle(article);
        }

        SearchProgress.IsVisible = false;
        return apiresult;
    }

    private async void SearchResults_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Article article) return;
        await Navigation.PushAsync(new DetailsArticlePage(article), true);
    }
}