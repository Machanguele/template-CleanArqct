namespace Application.Interfaces
{
    public interface ILocalizerHelper <T> where T: class
    {
        string GetString(string key, string param = "");
    }
}