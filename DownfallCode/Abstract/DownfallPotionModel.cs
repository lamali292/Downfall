using BaseLib.Extensions;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) : 
    ConstructedPotionModel(potionRarity, potionUsage, targetType)
{
    protected string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();
    public override string CustomPackedImagePath => $"{IconName}.tres".DownfallTresPotionImagePath();
    public override string CustomPackedOutlinePath => $"{IconName}_outline.tres".DownfallTresPotionImagePath();
    
}

public abstract class DownfallPotionModel<T>(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) : 
    DownfallPotionModel(potionRarity, potionUsage, targetType)
where T : DownfallCharacterModel
{
    public override string CustomPackedImagePath => $"{IconName}.tres".TresPotionImagePath<T>();
    public override string CustomPackedOutlinePath => $"{IconName}_outline.tres".TresPotionImagePath<T>();
}