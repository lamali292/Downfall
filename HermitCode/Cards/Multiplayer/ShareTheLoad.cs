using BaseLib.Utils;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class ShareTheLoad : HermitCardModel
{
    public ShareTheLoad() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(10, 4);
        WithCards(2, 1);
        WithEnergy(1);
        WithKeyword(CardKeyword.Exhaust);
        WithDeadOn();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await HermitCmd.DeadOn(ctx, this, cardPlay, async () =>
        {
            foreach (var player in Owner.OtherTeammates)
            {
                await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
                // TODO : use DrawWithoutBlockingOnOtherPlayers here on main/beta merge.
                await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, player);
            }
        });
    }
}