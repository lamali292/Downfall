using BaseLib.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallPotionModel : ConstructedPotionModel
{
    protected DownfallPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType) : base(potionRarity, potionUsage, targetType)
    {
        WithTips(e => e is DownfallPotionModel { Artist: not null } card ? [card.Artist.HoverTip] : []);
    }
    
    protected string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    protected virtual Artist? Artist => null;
    
    public override string CustomPackedImagePath => $"{IconName}.tres".DownfallTresPotionImagePath();
    public override string CustomPackedOutlinePath => $"{IconName}_outline.tres".DownfallTresPotionImagePath();
}

public abstract class DownfallPotionModel<T>(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType)
    :
        DownfallPotionModel(potionRarity, potionUsage, targetType)
    where T : DownfallCharacterModel
{
    public override string CustomPackedImagePath => $"{IconName}.tres".TresPotionImagePath<T>();
    public override string CustomPackedOutlinePath => $"{IconName}_outline.tres".TresPotionImagePath<T>();
}