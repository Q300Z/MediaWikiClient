using System.Diagnostics;
using MediaWikiClient.Factories;
using MediaWikiClient.Models;
using MediaWikiClient.Services;

namespace MediaWikiClient;

public partial class SearchPage
{
    private readonly IDataService _dataService;
    private readonly IMediaWikiApi _mediaWikiApi;
    private List<Article> _apiresult = new();

    public SearchPage()
    {
        InitializeComponent();
        _dataService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IDataService>();
        _mediaWikiApi = Application.Current.MainPage.Handler.MauiContext.Services.GetService<IMediaWikiApi>();
        // BindingContext = this;

        SearchResults.Opacity = 0;
        SearchResults.ItemsSource = null;
        NbResults.Text = "Aucun résultat";
        NbResults.Opacity = 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SearchBar.Focus();
    }

    private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchBar = (SearchBar)sender;
        PopulateSearchResults(searchBar.Text);
    }

    private void SearchBar_OnSearchButtonPressed(object sender, EventArgs e)
    {
        var searchBar = (SearchBar)sender;
        PopulateSearchResults(searchBar.Text);
    }


    private async void PopulateSearchResults(string searchBar)
    {
        if (searchBar.Trim().Length > 0)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var dbresult = await _dataService.SearchArticle(searchBar.Trim());
                if (dbresult != null && dbresult.Count >= 10)
                {
                    _apiresult.AddRange(dbresult.Where(art => _apiresult.All(existing => existing.Titre != art.Titre)));
                    SearchResults.ItemsSource = _apiresult.OrderByDescending(art => art.Titre.StartsWith(searchBar.Trim())).ThenBy(art => art.Titre.StartsWith(searchBar.Trim())).ToList();
                }
                else
                {
                    SearchResults.ItemsSource = await GetArticlesFromApi(searchBar.Trim());
                }

                stopwatch.Stop();

                var nbresults = ((List<Article>)SearchResults.ItemsSource)?.Count ?? 0;
                if (nbresults > 0)
                {
                    await NbResults.FadeTo(0.75, 240);
                    NbResults.Text = $"{nbresults} résultats en {stopwatch.ElapsedMilliseconds.ToString()} ms";
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        await Titre.FadeTo(0);
                        await SousTitre.FadeTo(0);
                        Titre.IsVisible = false;
                        SousTitre.IsVisible = false;
                    }
                }

                await SearchResults.FadeTo(1);
            }
            catch (Exception exep)
            {
                Console.Error.WriteLine(exep);
                SearchProgress.IsVisible = false;
                _apiresult = new List<Article>();
                SearchResults.ItemsSource = null;
                await DisplayAlert("Erreur", $"Une erreur est survenue lors de la recherche : \n {exep}", "OK");
            }
        }
        else
        {
            _apiresult = new List<Article>();
            SearchResults.ItemsSource = null;
            SearchProgress.IsVisible = false;
            await SearchResults.FadeTo(0);
            NbResults.Text = "Aucun résultat";
            await NbResults.FadeTo(0, 1250);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                Titre.IsVisible = true;
                SousTitre.IsVisible = true;
                await Titre.FadeTo(1);
                await SousTitre.FadeTo(1);
            }
        }
    }

    private async Task<List<Article>> GetArticlesFromApi(string search)
    {
        try
        {
            SearchProgress.Progress = 0;
            SearchProgress.IsVisible = true;

            var apiResult = await _mediaWikiApi.RechercherArticle(search);

            _apiresult = _apiresult.Where(art => art.Titre.Contains(search)).ToList();

            foreach (var article in apiResult.Where(article => _apiresult.All(existing => existing.Titre != article.Titre)))
            {
                await SearchProgress.ProgressTo((double)apiResult.IndexOf(article) / apiResult.Count, 250, Easing.Linear);
                article.InDatabase = true;
                article.DateInDatabase = DateTime.Now;
                await _dataService.AddArticle(article);
                article.InDatabase = false;

                _apiresult.Add(article);
            }

            _apiresult = _apiresult.OrderByDescending(art => art.Titre.StartsWith(search)).ThenBy(art => art.Titre.StartsWith(search)).ToList();

            SearchProgress.IsVisible = false;

            return _apiresult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la recherche d'articles : {ex.Message}");
            await DisplayAlert("Erreur", $"Erreur lors de la recherche d'articles : \n {ex.Message}", "OK");
            SearchProgress.IsVisible = false;
            return null;
        }
    }

    private async void SearchResults_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Article article) return;
        await Navigation.PushAsync(new DetailsArticlePage(article), true);
    }
}