using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Daggerstorm : AwakenedCardModel
{
    public Daggerstorm() : base(2, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        this.WithPower<DaggerstormPower>(4, 2, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DaggerstormPower>(ctx, this);
    }
}