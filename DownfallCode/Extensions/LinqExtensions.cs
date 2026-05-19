namespace Downfall.DownfallCode.Extensions;

public static class LinqExtensions
{
    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        foreach (var item in source.ToList())
            await action(item);
    }
}