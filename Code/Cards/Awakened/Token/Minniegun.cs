using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Minniegun : AwakenedCardModel
{
    public Minniegun() : base(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(2);
        WithVars(new RepeatVar(5));
        WithTip(typeof(Void));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.Repeat.IntValue).Execute(ctx);
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Draw, CardPilePosition.Random, 0.3f);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}