using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.DownfallCode.Utils.UI;

public interface ITopBarElementDescriptor
{
    string ScenePath { get; }
    float Width { get; }
    bool CanUse(Player player);
}

public interface ITopBarElement
{
    void Initialize(Player player);
}