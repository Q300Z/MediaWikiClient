using System.ComponentModel;

namespace MediaWikiClient.Models;

public class Article : INotifyPropertyChanged
{
    private string contenu;

    private bool inDatabase;

    private bool isFavoris;

    private bool isLu;

    public Article(int id, string titre, string resumer, DateTime date, bool indatabase)
    {
        Id = id;
        Titre = titre;
        Resumer = resumer;
        Date = date;
        InDatabase = indatabase;
        IsFavoris = false;
        DateFavoris = DateTime.Now;
        IsLu = false;
        DateLu = DateTime.Now;
        Contenu = "";
    }

    public Article(int id, string titre, string resumer, DateTime date, bool inDatabase, DateTime dateInDatabase, bool isFavoris, DateTime
            dateFavoris, bool isLu, DateTime dateLu,
        string contenu)
    {
        Id = id;
        Titre = titre;
        Resumer = resumer;
        Date = date;
        InDatabase = inDatabase;
        DateInDatabase = dateInDatabase;
        IsFavoris = isFavoris;
        DateFavoris = dateFavoris;
        IsLu = isLu;
        DateLu = dateLu;
        Contenu = contenu;
    }

    public int Id { get; init; }
    public string Titre { get; init; }
    public string Resumer { get; init; }
    public DateTime Date { get; init; }
    public string Url => $"https://fr.wikipedia.org/?curid={Id}";

    public bool InDatabase
    {
        get => inDatabase;
        set
        {
            if (inDatabase != value)
            {
                inDatabase = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateInDatabase { get; set; }

    public bool IsFavoris
    {
        get => isFavoris;
        set
        {
            if (isFavoris != value)
            {
                isFavoris = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateFavoris { get; set; }

    public bool IsLu
    {
        get => isLu;
        set
        {
            if (isLu != value)
            {
                isLu = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateLu { get; set; }

    public string Contenu
    {
        get => contenu;
        set
        {
            if (contenu != value)
            {
                contenu = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}