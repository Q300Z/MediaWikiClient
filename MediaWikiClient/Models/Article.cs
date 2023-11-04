using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaWikiClient.Models;

public class Article : INotifyPropertyChanged
{
    public int Id { get; init; }
    public string Titre { get; init; }
    public string Resumer { get; init; }
    public string Timestamp { get; init; }
    public bool InDatabase { get; init; }
    public string Url => $"https://fr.wikipedia.org/?curid={Id}";
    private string contenu;

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

    private void OnPropertyChanged(string name =null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}