using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Brainshock : AwakenedCardModel
{
    public Brainshock() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        this.WithTip<Void>();
    }

    protected override Artist Artist => Artist.Get<Occultpyromancer>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Discard);
    }
}