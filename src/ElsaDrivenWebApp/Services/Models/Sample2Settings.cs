namespace ElsaDrivenWebApp.Services.Models
{
    public class Envelope<T>  where T : class
    {
        public Envelope(T settings)
        {
            Input = settings;
        }
        public T Input { get; }

    }

    public class Sample2Settings
    {
        public string Name { get; set; }
    }
}
