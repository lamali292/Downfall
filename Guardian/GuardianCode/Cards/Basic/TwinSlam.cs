using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class TwinSlam : GuardianCardModel
{
    public TwinSlam() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithUpgradedCardTip<SecondSlam>((c, g) =>
        {
            if (g is GuardianCardModel other)
                c.AddGems(other.Gems);
        });
    }

    public override int GemSlots => IsUpgraded ? 2 : 1;


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card =
            await DownfallCardCmd.GiveCard<SecondSlam>(Owner, PileType.Hand, upgraded: IsUpgraded) as GuardianCardModel;
        if (card == null) return;
        card.AddGems(Gems);
        NCard.FindOnTable(card)?.ReloadOverlay();
        ;
    }
}