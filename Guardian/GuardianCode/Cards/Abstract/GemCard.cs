using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Gems;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards.Abstract;

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
    protected GemCard() : base(0, GuardianCardType.Gem, CardRarity.None, TargetType.None)
    {
        _titleLocString = GuardianModelDb.Gem<T>().TitleLocString;
        WithKeyword(GuardianKeyword.Gem);
        foreach (var extraHoverTip in GuardianModelDb.Gem<T>().ExtraHoverTips)
            WithTip(new TooltipSource(_ => extraHoverTip));
    }

    public override CardRarity Rarity => GuardianModelDb.Gem<T>().Rarity;

    public override int MaxUpgradeLevel => 0;

    public GemModel GemModel => GuardianModelDb.Gem<T>();
    public LocString GemDescription => GuardianModelDb.Gem<T>().Description;

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