using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Prepare : SlimeBossCardModel
{
    public Prepare() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(10, 5);
        WithEnergy(1);
        this.WithPower<EnergyNextTurnPower>(1, false);
        this.WithPower<DrawCardsNextTurnPower>(2, false);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
    }
}