namespace Champ.ChampCode.Interfaces;

public interface IFinisherCard
{
    /// <summary>The Finisher's declared rules; see <see cref="FinisherDescriptor"/>.</summary>
    FinisherDescriptor Finisher { get; }
}