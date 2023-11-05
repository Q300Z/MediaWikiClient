using MediaWikiClient.Models;
using Newtonsoft.Json.Linq;

namespace MediaWikiClient.Services;

public interface IMediaWikiApi
{
    Task<bool> TestConnection();
    Task<List<Article>> RechercherArticle(string termeRecherche);
    Task<string> DetailsArticle(int pageId);
}

public class MediaWikiApi : IMediaWikiApi
{
    private readonly Constants _constants = new();
    private readonly HttpClient _httpClient = new();
    private readonly string _apiUrl; // URL de l'API de Wikipédia en français

    public MediaWikiApi()
    {
        _apiUrl = _constants.EndpointApi;
    }

    public async Task<bool> TestConnection()
    {
        try
        {
            var testEndpoint = $"{_apiUrl}?action=query&meta=siteinfo&siprop=general&format=json";
            var response = await _httpClient.GetStringAsync(testEndpoint);

            if (!string.IsNullOrEmpty(response))
            {
                Console.WriteLine(response);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync("Erreur de connexion à l'API MediaWiki : " + ex.Message);
            return false;
        }
    }

    public async Task<List<Article>> RechercherArticle(string termeRecherche)
    {
        // Effectuer une recherche
        var rechercheEndpoint = $"{_apiUrl}?action=query&list=search&srsearch={termeRecherche}&format=json";
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
        // var detailsEndpoint = $"{_apiUrl}?action=query&pageids={pageId}&prop=info|revisions&inprop=url&rvprop=content&format=json&formatversion=2";
        var detailsEndpoint = $"{_apiUrl}?action=parse&format=json&pageid={pageId}&formatversion=2";
        var detailsResultat = await _httpClient.GetStringAsync(detailsEndpoint);

        // Analyser les résultats de la recherche
        var detailsJson = JObject.Parse(detailsResultat);

        //var contenu = (string)detailsJson["query"]["pages"][0]["revisions"][0]["content"];
        var contenu = (string)detailsJson["parse"]["text"];

        return contenu;
    }
}