using MediaWikiClient.Models;

namespace MediaWikiClient;

public partial class WebViewPage
{
    public WebViewPage(Article article)
    {
        InitializeComponent();
        BindingContext = article;
        WebView.Source = article.Url;
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var article = (Article)button.BindingContext;
        await Launcher.OpenAsync(article.Url);
        await Navigation.PopAsync(true);
    }
}