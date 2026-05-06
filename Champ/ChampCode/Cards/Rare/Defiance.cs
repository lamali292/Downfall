using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class Defiance : ChampCardModel
{
    public Defiance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Retain);
        WithCalculatedBlock(0, CalcBlock);
    }

    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        return card.Owner.Creature.GetPowerAmount<CounterPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.CardCalculatedBlock(this, cardPlay);
        await PowerCmd.Remove<CounterPower>(Owner.Creature);
    }
}