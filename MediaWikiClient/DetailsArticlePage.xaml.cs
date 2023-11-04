using MediaWikiClient.Models;
using MediaWikiClient.Services;

namespace MediaWikiClient;

//https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm
public partial class DetailsArticlePage : ContentPage
{
    private readonly MediaWikiApi _mediaWikiApi;

    public DetailsArticlePage(Article article)
    {
        InitializeComponent();
        BindingContext = article;
        _mediaWikiApi = Application.Current.MainPage.Handler.MauiContext.Services.GetService<MediaWikiApi>();


        LoadContenu(article);
    }

    private async void LoadContenu(Article article)
    {
        try
        {
            var contenu = await _mediaWikiApi.DetailsArticle(article.Id);

            article.Contenu = contenu;
            
            var htmlSource = new HtmlWebViewSource
            {
                Html = article.Contenu
            };
            ContenuWebview.Source = htmlSource;
        }
        catch (Exception e)
        {
            await DisplayAlert("Erreur", "Impossible de charger le contenu de l'article : " + e.Message, "OK");
        }
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var article = (Article)button.BindingContext;
        await Navigation.PushAsync(new WebViewPage(article), true);
    }
}