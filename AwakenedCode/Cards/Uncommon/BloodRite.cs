using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class BloodRite : AwakenedCardModel
{
    public BloodRite() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        this.WithTip<Ceremony>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await DownfallCardCmd.GiveCard<Ceremony>(Owner, PileType.Hand);
    }
}