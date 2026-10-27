using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class SwordOfFrost : Ruina2Card
{
    protected override MonsterModel? OverlayMonster => GetEncounterModel()?.AllPossibleMonsters.LastOrDefault();
    public SwordOfFrost() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, "SNOW_QUEEN_ELITE")
    {
        WithTip<Burn>();
        WithTip<Ember>();
        WithPower<SwordOfFrostPower>(1, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SwordOfFrostPower>(ctx, this);
    }
}