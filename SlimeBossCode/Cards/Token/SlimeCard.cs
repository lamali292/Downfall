using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public abstract class SlimeCard<T>
    : SlimeBossCardModel, ISlimeCard, IModfyCardDescription
    where T : SlimeModel
{
    protected SlimeCard(bool showInCardLibrary = true, bool autoAdd = true) : base(-1, CardType.Skill, CardRarity.Token,
        TargetType.Self, showInCardLibrary, autoAdd)
    {
        WithTips(_ => [SlimeModel.SlimeTip]);
    }

    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;

    protected override bool IsPlayable => false;
    public override string Title => SlimeModel.Title.GetFormattedText();

    public LocString ModifyDescription(LocString oldLocString)
    {
        var description = new LocString("cards", "SLIMEBOSS-SLIME_CARD.description");
        description.Add("Slime", SlimeModel.Title.GetFormattedText());
        return description;
    }

    public SlimeModel SlimeModel => ModelDb.Get<T>();
}

public interface ISlimeCard
{
    SlimeModel SlimeModel { get; }
}

#pragma warning disable

// Uncommon Slimes

public class SlimeCardBruiser : SlimeCard<BruiserSlime>;
public class SlimeCardMuscle : SlimeCard<MuscleSlime>;
public class SlimeCardTaunting : SlimeCard<TauntingSlime>;
public class SlimeCardPsychic : SlimeCard<PsychicSlime>;
public class SlimeCardLeeching : SlimeCard<LeechingSlime>;
public class SlimeCardSpike : SlimeCard<SpikeSlime>;
public class SlimeCardCultist : SlimeCard<CultistSlime>;
public class SlimeCardGuerilla : SlimeCard<GuerillaSlime>;

// rare Slimes
public class SlimeCardMassive : SlimeCard<MassiveSlime>;
public class SlimeCardEvolution : SlimeCard<EvolutionSlime>;
public class SlimeCardRoyal : SlimeCard<RoyalSlime>;
public class SlimeCardDarkling : SlimeCard<DarklingSlime>;


// Unused Slimes
[Obsolete]
public class SlimeCardGhostflame : SlimeCard<GhostflameSlime>;

[Obsolete]
public class SlimeCardAncient : SlimeCard<AncientSlime>;

[Obsolete]
public class SlimeCardBronze : SlimeCard<BronzeSlime>;

[Obsolete]
public class SlimeCardTime : SlimeCard<TimeSlime>;

[Obsolete]
public class SlimeCardInsulting : SlimeCard<InsultingSlime>;

[Obsolete]
public class SlimeCardTorchhead : SlimeCard<TorchheadSlime>;

[Obsolete]
public class SlimeCardGreed() : SlimeCard<GreedSlime>(false, false);

[Obsolete]
public class SlimeCardScrap() : SlimeCard<ScrapSlime>(false, false);

[Obsolete]
public class SlimeCardMire : SlimeCard<MireSlime>;
#pragma warning restore