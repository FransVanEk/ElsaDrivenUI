using Elsa.Services;

namespace UserTask.AddOns.Bookmarks
{
    public class UserTaskSignalBookmark : IBookmark
    {
        public string Signal { get; set; } = default!;
    }
}