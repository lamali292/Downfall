using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class ShareTheLoad : HermitCardModel,IHasDeadOnEffect
{
    public ShareTheLoad() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(10, 4);
        WithCards(2, 1);
        WithEnergy(1);
        WithKeyword(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var player in Owner.GetOtherPlayers())
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
            // TODO : use DrawWithoutBlockingOnOtherPlayers here on main/beta merge.
            await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, player);
        }
            
    }
}