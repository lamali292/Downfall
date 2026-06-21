using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Minniegun : AwakenedCardModel
{
    public Minniegun() : base(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(2);
        this.WithRepeat(5, 1);
        this.WithTip<Void>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue).Execute(ctx);
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Draw, CardPilePosition.Random, false, 0.3f);
    }
}