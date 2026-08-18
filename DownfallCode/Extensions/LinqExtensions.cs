namespace Downfall.DownfallCode.Extensions;

public static class LinqExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public async Task ForEachAsync(Func<T, Task> action)
        {
            foreach (var item in source.ToList())
                await action(item);
        }
    }
}