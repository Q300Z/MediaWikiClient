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

    private async void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchBar = (SearchBar)sender;
        if (searchBar.Text.Trim().Length > 0)
        {
            SearchResults.ItemsSource = await _mediaWikiApi.RechercherArticle(searchBar.Text.Trim());
            await SearchResults.FadeTo(1);
        }
        else
        {
            SearchResults.ItemsSource = null;
            await SearchResults.FadeTo(0);
        }
    }

    private async void SearchResults_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Article article) return;
        await Navigation.PushAsync(new DetailsArticlePage(article), true);
    }
}