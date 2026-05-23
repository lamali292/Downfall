using Hermit.HermitCode.Powers;

namespace Hermit.HermitCode.Events;

public interface IShouldPreventBruiseRemoval
{
    bool ShouldPreventBruiseRemoval(BruisePower power);

    Task AfterPreventedBruiseRemoval(BruisePower power)
    {
        return Task.CompletedTask;
    }
}