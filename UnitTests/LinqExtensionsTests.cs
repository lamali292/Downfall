using Downfall.DownfallCode.Extensions;
using Xunit;

namespace Downfall.UnitTests;

public class LinqExtensionsTests
{
    [Fact]
    public async Task ForEachAsync_RunsActionsSequentiallyInOrder()
    {
        var log = new List<string>();

        await new[] { 1, 2, 3 }.ForEachAsync(async i =>
        {
            log.Add($"start{i}");
            await Task.Delay(5);
            log.Add($"end{i}");
        });

        Assert.Equal(["start1", "end1", "start2", "end2", "start3", "end3"], log);
    }

    [Fact]
    public async Task ForEachAsync_IteratesSnapshot_SoSourceMayBeMutated()
    {
        var items = new List<int> { 1, 2 };
        var seen = new List<int>();

        await items.ForEachAsync(i =>
        {
            items.Add(i + 10);
            seen.Add(i);
            return Task.CompletedTask;
        });

        Assert.Equal([1, 2], seen);
    }
}
