using MediaWikiClient.Factories;
using MediaWikiClient.Models;
using MediaWikiClient.Services;

namespace MediaWikiClient;

//https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm
public partial class DetailsArticlePage : ContentPage
{
    private readonly IDataService _dataService;
    private readonly MediaWikiApi _mediaWikiApi;

    public DetailsArticlePage(Article article)
    {
        InitializeComponent();
        BindingContext = article;
        _dataService = Application.Current.MainPage.Handler.MauiContext.Services.GetService<IDataService>();
        _mediaWikiApi = Application.Current.MainPage.Handler.MauiContext.Services.GetService<MediaWikiApi>();



        ButtonFavoris.Text = article.IsFavoris ? "Retirer des favoris" : "Ajouter aux favoris";
        ButtonFavoris.ImageSource = article.IsFavoris ? "starfilled.png" : "star.png";
        
    }
    

    private async void LoadContenu(Article article)
    {
        try
        {
            if (article.Contenu == "" || article.IsLu == false)
            {
                var contenu = await _mediaWikiApi.DetailsArticle(article.Id);
                article.Contenu = contenu;
                article.IsLu = true;
                article.DateLu = DateTime.Now;
                await _dataService.UpdateArticle(article);
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var htmlSource = new HtmlWebViewSource
                {
                    Html = article.Contenu ?? "<h1>Impossible de charger le contenu de l'article</h1>"
                };
                ContenuWebview.Source = htmlSource;
                await ContenuWebview.FadeTo(1, 500, Easing.CubicInOut);
            });
        }
        catch (Exception e)
        {
            await DisplayAlert("Erreur", "Impossible de charger le contenu de l'article : \n " + e.Message, "OK");
        }
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var article = (Article)button.BindingContext;
        await Navigation.PushAsync(new WebViewPage(article), true);
    }

    private void FavoriteBtn_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var article = (Article)button.BindingContext;

        article.IsFavoris = !article.IsFavoris;
        article.DateFavoris = DateTime.Now;
        _dataService.UpdateArticle(article);

        ButtonFavoris.Text = article.IsFavoris ? "Retirer des favoris" : "Ajouter aux favoris";
        ButtonFavoris.ImageSource = article.IsFavoris ? "starfilled.png" : "star.png";
    }

    private async void BtnSuppArticle_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Confirmation", "Voulez-vous vraiment supprimer l'article ?", "Oui", "Non");
        if (result)
        {
            var article = BindingContext as Article;
            if (await _dataService.DeleteArticle(article))
            {
                article = null;
                await DisplayAlert("Succès", "Article supprimé avec succès", "OK");
                await Navigation.PopAsync(true);
            }
        }
    }

    private void ContenuWebview_OnLoaded(object sender, EventArgs e)
    {
        LoadContenu(BindingContext as Article);
    }
}