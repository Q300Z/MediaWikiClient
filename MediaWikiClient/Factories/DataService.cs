using System.Data;
using MediaWikiClient.Models;
using Microsoft.Data.SqlClient;

namespace MediaWikiClient.Factories;

public interface IDataService
{
    List<Article> SearchArticle(string? search = null, int? limit = null);
    List<Article> GetArticle(int id);
    bool AddArticle(Article Article);
    bool UpdateArticle(Article Article);
    bool DeleteArticle(Article Article);
}

public class DataService : IDataService
{
    private readonly SqlConnection _sqlConnection;

    public DataService()
    {
        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = "192.168.1.103"; //"localhost";
        builder.UserID = "sa";
        builder.Password = "password@123";
        builder.InitialCatalog = "MediaWikiClient";
        builder.TrustServerCertificate = true;
        _sqlConnection = new SqlConnection(builder.ConnectionString);
    }


    public List<Article> SearchArticle(string? search = null, int? limit = null)
    {
        try
        {
            var sql =
                "SELECT TOP(@limit) Article.id, Article.nom, adresse, telephone, email, ville, cp, activite, url, moyen, date " +
                "FROM Article";
            List<Article> values;
            _sqlConnection.Open();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                command.Parameters.Add(new SqlParameter("@limit", limit ?? int.MaxValue));
                using (var reader = command.ExecuteReader())
                {
                    values = reader.Cast<IDataRecord>().Select(r => new Article(
                    )).ToList();
                    values.ForEach(async Article => { });
                }

                return values;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return new List<Article>();
        }
        finally
        {
            _sqlConnection.Close();
        }
    }

    public List<Article> GetArticle(int id)
    {
        try
        {
            var sql =
                "SELECT Article.id, Article.nom, adresse,telephone,email,ville,cp,activite,url,moyen,date " +
                "FROM Article " +
                "WHERE Article.id = ${id}";

            List<Article> values;
            _sqlConnection.Open();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                using (var reader = command.ExecuteReader())
                {
                    values = reader.Cast<IDataRecord>().Select(r => new Article(
                    )).ToList();
                    values.ForEach(async Article => { });
                }
            }

            return values;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return new List<Article>();
        }
        finally
        {
            _sqlConnection.Close();
        }
    }

    public bool AddArticle(Article Article)
    {
        try
        {
            var sql = $"INSERT INTO Article () VALUES " +
                      $"('')";

            _sqlConnection.Open();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                var result = command.ExecuteNonQuery() > 0 ? true : false;

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            _sqlConnection.Close();
        }
    }

    public bool UpdateArticle(Article Article)
    {
        try
        {
            var sql = "UPDATE Article SET ";

            _sqlConnection.Open();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                var result = command.ExecuteNonQuery() > 0 ? true : false;
                return result;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            _sqlConnection.Close();
        }
    }

    public bool DeleteArticle(Article Article)
    {
        try
        {
            var sql = $"DELETE FROM Article WHERE id = {Article.Id}";
            _sqlConnection.Open();
            using (var command = new SqlCommand(sql, _sqlConnection))
            {
                var result = command.ExecuteNonQuery() > 0 ? true : false;

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            _sqlConnection.Close();
        }
    }
}