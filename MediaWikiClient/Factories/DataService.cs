using System.Data;
using MediaWikiClient.Models;
using Microsoft.Data.SqlClient;

namespace MediaWikiClient.Factories;

public interface IDataService
{
    Task<bool> TestConnection();
    Task<List<Article>> SearchArticle(string search = "");
    Task<bool> AddArticle(Article article);
    Task<bool> UpdateArticle(Article article);
    Task<bool> DeleteArticle(Article article);
    Task<bool> ClearHistory();
    Task<bool> DropDatabase();
}

public class DataService : IDataService
{
    private readonly Constants _constants = new ();
    private readonly SqlConnection _sqlConnection;

    public DataService()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = _constants.DbAdresse, //"localhost";
            UserID = _constants.DbUsername, //"sa";
            Password = _constants.DbPassword, //"yourpassword";
            InitialCatalog = _constants.DbName, //"mediawiki";
            TrustServerCertificate = _constants.TrustServerCertificate, //true;
            MultipleActiveResultSets = true //Permet de faire plusieurs requêtes en même temps sur une même connexion
        };
        _sqlConnection = new SqlConnection(builder.ConnectionString);
    }

    public async Task<bool> TestConnection()
    {
        try
        {
            await _sqlConnection.OpenAsync();
            return true;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync("Erreur de connexion à la base de données : " + ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<List<Article>> SearchArticle(string search = "")
    {
        try
        {
            const string sql = "SELECT id, titre, resumer, contenu, date, inDatabase, dateInDatabase, isFavoris, dateFavoris, isLu, dateLu FROM articles WHERE titre LIKE @search ORDER BY titre";

            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.Add(new SqlParameter("@search", search + '%'));
                var articles = new List<Article>();
                await _sqlConnection.OpenAsync();
                using (var r = await command.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        var id = (int)r["id"];
                        var titre = (string)r["titre"];
                        var resumer = (string)r["resumer"];
                        var contenu = (string)r["contenu"];
                        var date = (DateTime)r["date"];
                        var inDatabase = (bool)r["indatabase"];
                        var dateInDatabase = (DateTime)r["dateindatabase"];
                        var isFavoris = (bool)r["isfavoris"];
                        var dateFavoris = (DateTime)r["datefavoris"];
                        var isLu = (bool)r["islu"];
                        var dateLu = (DateTime)r["datelu"];

                        articles.Add(new Article(id, titre, resumer, date, inDatabase, dateInDatabase, isFavoris, dateFavoris, isLu, dateLu,
                            contenu));
                    }
                }

                return articles;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return new List<Article>();
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<bool> AddArticle(Article article)
    {
        try
        {
            const string sql = "INSERT INTO articles (id, titre, resumer, contenu, date, indatabase, dateindatabase, isfavoris, datefavoris," +
                               "islu, datelu) VALUES (@Id, @Titre, @Resumer, @Contenu, @Date, @InDatabase, @DateInDatabase, @IsFavoris," +
                               " @DateFavoris, @IsLu, @DateLu)";

            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.AddWithValue("@Id", article.Id);
                command.Parameters.AddWithValue("@Titre", article.Titre);
                command.Parameters.AddWithValue("@Resumer", article.Resumer);
                command.Parameters.AddWithValue("@Contenu", article.Contenu);
                command.Parameters.AddWithValue("@Date", article.Date);
                command.Parameters.AddWithValue("@InDatabase", true);
                command.Parameters.AddWithValue("@DateInDatabase", article.DateInDatabase);
                command.Parameters.AddWithValue("@IsFavoris", article.IsFavoris);
                command.Parameters.AddWithValue("@DateFavoris", article.DateFavoris);
                command.Parameters.AddWithValue("@IsLu", article.IsLu);
                command.Parameters.AddWithValue("@DateLu", article.DateLu);
                await _sqlConnection.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<bool> UpdateArticle(Article article)
    {
        try
        {
            const string sql = "UPDATE articles SET titre = @Titre, resumer = @Resumer, contenu = @Contenu, date = @Date, indatabase = @InDatabase," +
                               "dateindatabase = @DateInDatabase, isfavoris = @IsFavoris, datefavoris = @DateFavoris, islu = @IsLu, datelu = @DateLu WHERE id = @Id";

            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.AddWithValue("@Id", article.Id);
                command.Parameters.AddWithValue("@Titre", article.Titre);
                command.Parameters.AddWithValue("@Resumer", article.Resumer);
                command.Parameters.AddWithValue("@Contenu", article.Contenu);
                command.Parameters.AddWithValue("@Date", article.Date);
                command.Parameters.AddWithValue("@InDatabase", article.InDatabase);
                command.Parameters.AddWithValue("@DateInDatabase", article.DateInDatabase);
                command.Parameters.AddWithValue("@IsFavoris", article.IsFavoris);
                command.Parameters.AddWithValue("@DateFavoris", article.DateFavoris);
                command.Parameters.AddWithValue("@IsLu", article.IsLu);
                command.Parameters.AddWithValue("@DateLu", article.DateLu);
                await _sqlConnection.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<bool> DeleteArticle(Article article)
    {
        try
        {
            const string sql = "DELETE FROM articles WHERE Id = @Id";

            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.AddWithValue("@Id", article.Id);
                if (_sqlConnection.State == ConnectionState.Closed)
                    await _sqlConnection.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<bool> ClearHistory()
    {
        try
        {
            const string sql = "update articles set islu = 0 where islu = 1";
            await _sqlConnection.OpenAsync();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }

    public async Task<bool> DropDatabase()
    {
        try
        {
            const string sql = "DELETE from articles";
            await _sqlConnection.OpenAsync();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return false;
        }
        finally
        {
            await _sqlConnection.CloseAsync();
        }
    }
}