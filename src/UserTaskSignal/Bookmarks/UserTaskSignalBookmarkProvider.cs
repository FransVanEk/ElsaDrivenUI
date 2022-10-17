using System.Runtime.CompilerServices;
using Elsa.Services;

namespace UserTask.AddOns.Bookmarks
{
    public class UserTaskSignalBookmarkProvider : BookmarkProvider<UserTaskSignalBookmark, UserTaskSignal>
    {
        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(
            BookmarkProviderContext<UserTaskSignal> context, CancellationToken cancellationToken) =>
            await GetBookmarksInternalAsync(context, cancellationToken).ToListAsync(cancellationToken);

        private async IAsyncEnumerable<BookmarkResult> GetBookmarksInternalAsync(
            BookmarkProviderContext<UserTaskSignal> context,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var signalName = (await context.ReadActivityPropertyAsync(x => x.Signal, cancellationToken))
                ?.ToLowerInvariant().Trim();

            // Can't do anything with an empty signal name.
            if (string.IsNullOrEmpty(signalName))
                yield break;

            yield return Result(new UserTaskSignalBookmark()
            {
                Signal = signalName
            });
        }
    }
}