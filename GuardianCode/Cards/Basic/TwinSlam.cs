using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Cards.Ancient;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class TwinSlam : GuardianCardModel, ITranscendenceCard, IGemSocketCard
{
    public TwinSlam() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(7);
        WithUpgradingCardTip<SecondSlam>((c, g) =>
        {
            if (g is IGemSocketCard other && c is IGemSocketCard t) t.AddGems(other.Gems.Select(e => e.CreateClone()));
        });
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public int GemSlots => IsUpgraded ? 2 : 1;

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<BaubleBurst>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = await DownfallCardCmd.GiveCard<SecondSlam>(Owner, PileType.Hand, upgraded: IsUpgraded);
        if (card is not IGemSocketCard secondSlam || this is not IGemSocketCard twinSlam) return;
        var gemClones = twinSlam.Gems.Select(originalGem => originalGem.CreateClone()).ToList();
        secondSlam.AddGems(gemClones);
        NCard.FindOnTable(card)?.ReloadOverlay();
    }
}