using MediaWikiClient.Models;
using Newtonsoft.Json.Linq;

namespace MediaWikiClient.Services;

public class MediaWikiApi
{
    private const string ApiUrl = "https://fr.wikipedia.org/w/api.php"; // URL de l'API de Wikipédia en français

    private readonly HttpClient _httpClient = new();

    public async Task<List<Article>> RechercherArticle(string termeRecherche)
    {
        // Effectuer une recherche
        var rechercheEndpoint = $"{ApiUrl}?action=query&list=search&srsearch={termeRecherche}&format=json";
        var rechercheResultat = await _httpClient.GetStringAsync(rechercheEndpoint);

        // Analyser les résultats de la recherche
        var resultatJson = JObject.Parse(rechercheResultat);
        var articlesJson = (JArray)resultatJson["query"]["search"];

        // Convertir les résultats de la recherche en une liste d'articles
        var articles = new List<Article>();

        foreach (var articleJson in articlesJson)
        {
            var article = new Article
            (
                (int)articleJson["pageid"],
                (string)articleJson["title"],
                (string)articleJson["snippet"],
                (DateTime)articleJson["timestamp"],
                false
            );
            articles.Add(article);
        }

        return articles;
    }

    public async Task<string> DetailsArticle(int pageId)
    {
        // Obtenir les détails d'un article en utilisant son ID de page
        // var detailsEndpoint = $"{ApiUrl}?action=query&pageids={pageId}&prop=info|revisions&inprop=url&rvprop=content&format=json&formatversion=2";
        var detailsEndpoint = $"{ApiUrl}?action=parse&format=json&pageid={pageId}&formatversion=2";
        var detailsResultat = await _httpClient.GetStringAsync(detailsEndpoint);

        // Analyser les résultats de la recherche
        var detailsJson = JObject.Parse(detailsResultat);

        //var contenu = (string)detailsJson["query"]["pages"][0]["revisions"][0]["content"];
        var contenu = (string)detailsJson["parse"]["text"];

        return contenu;
    }
}