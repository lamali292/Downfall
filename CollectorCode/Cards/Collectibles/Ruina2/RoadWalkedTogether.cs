using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Collectibles.Ruina2;

public class RoadWalkedTogether : Ruina2Card
{
    protected override MonsterModel? OverlayMonster => GetEncounterModel()?.AllPossibleMonsters.LastOrDefault();
    public RoadWalkedTogether() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "ROAD_HOME_ELITE", 0.84f)
    {
        WithBlock(12, 3);
        WithPower<RoadWalkedTogetherPower>(2,1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Apply<RoadWalkedTogetherPower>(ctx, this, cardPlay);
    }
}