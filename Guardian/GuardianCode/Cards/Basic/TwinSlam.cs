using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Cards.Ancient;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class TwinSlam : GuardianCardModel, ITranscendenceCard
{
    public TwinSlam() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithUpgradedCardTip<SecondSlam>((c, g) =>
        {
            if (g is GuardianCardModel other) c.AddGems(other.Gems.Select(e => e.CreateClone()));
        });
    }

    public override int GemSlots => IsUpgraded ? 2 : 1;


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (await DownfallCardCmd.GiveCard<SecondSlam>(Owner, PileType.Hand, upgraded: IsUpgraded) is not
            GuardianCardModel card) return;
        var gemClones = Gems.Select(originalGem => originalGem.CreateClone()).ToList();
        card.AddGems(gemClones);
        NCard.FindOnTable(card)?.ReloadOverlay();
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<BaubleBurst>();
    }
}