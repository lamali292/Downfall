using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public abstract class SlimeCard<T>(bool showInCardLibrary = true, bool autoAdd = true)
    : SlimeBossCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self, showInCardLibrary, autoAdd), ISlimeCard
    where T : SlimeModel
{
    protected override bool IsPlayable => false;
    public override string Title => SlimeModel.Title.GetFormattedText();
    public SlimeModel SlimeModel => ModelDb.Get<T>();
}

public interface ISlimeCard
{
    SlimeModel SlimeModel { get; }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
public static class CardModelDescriptionPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is not ISlimeCard slimeCard) return;
        var description = new LocString("cards", "SLIMEBOSS-SLIME_CARD.description");
        description.Add("Slime", slimeCard.SlimeModel.Title.GetFormattedText());
        __result = description;
    }
}

#pragma warning disable

// Normal Slimes
public class SlimeCardLeeching : SlimeCard<LeechingSlime>;

public class SlimeCardMire : SlimeCard<MireSlime>;

public class SlimeCardBruiser : SlimeCard<BruiserSlime>;

public class SlimeCardGuerilla : SlimeCard<GuerillaSlime>;

// Specialist Slimes
public class SlimeCardAncient : SlimeCard<AncientSlime>;

public class SlimeCardBronze : SlimeCard<BronzeSlime>;

public class SlimeCardCultist : SlimeCard<CultistSlime>;

public class SlimeCardGhostflame : SlimeCard<GhostflameSlime>;

public class SlimeCardInsulting : SlimeCard<InsultingSlime>;

public class SlimeCardSpiky : SlimeCard<SpikySlime>;

public class SlimeCardTime : SlimeCard<TimeSlime>;

public class SlimeCardTorchhead : SlimeCard<TorchheadSlime>;

// Unused Slimes
[Obsolete]
public class SlimeCardGreed() : SlimeCard<GreedSlime>(false, false);

[Obsolete]
public class SlimeCardDarkling() : SlimeCard<DarklingSlime>(false, false);

[Obsolete]
public class SlimeCardScrap() : SlimeCard<ScrapSlime>(false, false);

#pragma warning restore