namespace MediaWikiClient.Models;

public class ArticleGroup : List<Article>
{
    public ArticleGroup(bool isFavorite, List<Article> articles) : base(articles)
    {
        IsFavorite = isFavorite;
        TitreFavorite = isFavorite ? "Favoris" : "Historique";
    }

    //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/grouping
    public bool IsFavorite { get; private set; }
    public string TitreFavorite { get; private set; }
}