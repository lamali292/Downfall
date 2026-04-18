using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Collector.Token;
using Downfall.Code.Core;
using Downfall.Code.Core.Guardian;
using Downfall.Code.CustomEnums;
using Downfall.Code.Gems;
using Downfall.Code.Keywords;
using Downfall.Code.Patches;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;


namespace Downfall.Code.Cards.Guardian.Abstract;

#pragma warning disable STS001
[Pool(typeof(GuardianCardPool))]
public class FragmentedGemCard() : GemCard<FragmentedGem>(CardRarity.Common);

[Pool(typeof(GuardianCardPool))]
public class Ruby() : GemCard<RubyGem>(CardRarity.Common);
[Pool(typeof(GuardianCardPool))]
public class Sapphire() : GemCard<SapphireGem>(CardRarity.Common);
[Pool(typeof(GuardianCardPool))]
public class Tourmaline() : GemCard<TourmalineGem>(CardRarity.Common);

[Pool(typeof(GuardianCardPool))]
public class Amber() : GemCard<AmberGem>(CardRarity.Uncommon);
[Pool(typeof(GuardianCardPool))]
public class Amethyst() : GemCard<AmethystGem>(CardRarity.Uncommon);
[Pool(typeof(GuardianCardPool))]
public class Aquamarine() : GemCard<AquamarineGem>(CardRarity.Uncommon);
[Pool(typeof(GuardianCardPool))]
public class Emerald() : GemCard<EmeraldGem>(CardRarity.Uncommon);
[Pool(typeof(GuardianCardPool))]
public class Garnet() : GemCard<GarnetGem>(CardRarity.Uncommon);
[Pool(typeof(GuardianCardPool))]
public class Opal() : GemCard<OpalGem>(CardRarity.Uncommon);

[Pool(typeof(GuardianCardPool))]
public class Citrine() : GemCard<CitrineGem>(CardRarity.Rare);
[Pool(typeof(GuardianCardPool))]
public class Onyx() : GemCard<OnyxGem>(CardRarity.Rare);


#pragma warning restore STS001

public abstract class GemCard<T> : GuardianCardModel, IGemCard
where T : GemModel
{
    protected GemCard(CardRarity rarity) : base(0, CustomCardType.Gem, rarity, TargetType.None)
    {
        _titleLocString = DownfallModelDb.Gem<T>().TitleLocString;
        WithKeyword(DownfallKeywords.Gem);
    }
    
    
    
    public override int MaxUpgradeLevel => 0;
    protected sealed override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => GemModel.OnPlay(ctx, cardPlay);

    public GemModel GemModel => DownfallModelDb.Gem<T>();
    public LocString GemDescription => DownfallModelDb.Gem<T>().Description;

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
