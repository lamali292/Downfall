using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core;
using Downfall.Code.Core.Guardian;
using Downfall.Code.CustomEnums;
using Downfall.Code.Gems;
using Downfall.Code.Keywords;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Guardian.Abstract;

#pragma warning disable STS001
[Pool(typeof(GuardianCardPool))]
public class FragmentedGemCard : GemCard<FragmentedGem>;

[Pool(typeof(GuardianCardPool))]
public class Ruby : GemCard<RubyGem>;

[Pool(typeof(GuardianCardPool))]
public class Sapphire : GemCard<SapphireGem>;

[Pool(typeof(GuardianCardPool))]
public class Tourmaline : GemCard<TourmalineGem>;

[Pool(typeof(GuardianCardPool))]
public class Amber : GemCard<AmberGem>;

[Pool(typeof(GuardianCardPool))]
public class Amethyst : GemCard<AmethystGem>;

[Pool(typeof(GuardianCardPool))]
public class Aquamarine : GemCard<AquamarineGem>;

[Pool(typeof(GuardianCardPool))]
public class Emerald : GemCard<EmeraldGem>;

[Pool(typeof(GuardianCardPool))]
public class Garnet : GemCard<GarnetGem>;

[Pool(typeof(GuardianCardPool))]
public class Opal : GemCard<OpalGem>;

[Pool(typeof(GuardianCardPool))]
public class Citrine : GemCard<CitrineGem>;

[Pool(typeof(GuardianCardPool))]
public class Onyx : GemCard<OnyxGem>;

#pragma warning restore STS001

public abstract class GemCard<T> : GuardianCardModel, IGemCard
    where T : GemModel
{
    protected GemCard() : base(0, CustomCardType.Gem, CardRarity.None, TargetType.None)
    {
        _titleLocString = DownfallModelDb.Gem<T>().TitleLocString;
        WithKeyword(DownfallKeywords.Gem);
        foreach (var extraHoverTip in DownfallModelDb.Gem<T>().ExtraHoverTips)
            WithTip(new TooltipSource(_ => extraHoverTip));
    }

    public override CardRarity Rarity => DownfallModelDb.Gem<T>().Rarity;

    public override int MaxUpgradeLevel => 0;

    public GemModel GemModel => DownfallModelDb.Gem<T>();
    public LocString GemDescription => DownfallModelDb.Gem<T>().Description;

    protected sealed override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return GemModel.OnPlay(ctx, cardPlay);
    }
}

public interface IGemCard
{
    LocString GemDescription { get; }
    GemModel GemModel { get; }
}

[HarmonyPatch(typeof(CardModel), "get_Description")]
public static class GemCardTitlePatch
{
    private static bool Prefix(CardModel __instance, ref LocString __result)
    {
        if (__instance is not IGemCard gem) return true;
        __result = gem.GemDescription;
        return false;
    }
}