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

    public class SampleSettings
    {
        public string Name { get; set; }
    }
}
