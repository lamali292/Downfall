using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class HeatShield : HexaghostCardModel
{
    public HeatShield() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Exhaust);
        WithCalculatedBlock(0, Calc);
        this.WithTip<SoulBurnPower>();
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    private static decimal Calc(CardModel arg1, Creature? creature)
    {
        return creature?.GetPowerAmount<SoulBurnPower>() ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}