using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Guardian.Token;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class SentryBlast : GuardianCardModel
{
    public SentryBlast() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithUpgradedCardTip<SentryWave>();
        WithTip(DownfallTip.Stasis);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var card = CombatState!.CreateCard(ModelDb.Card<SentryWave>(), Owner);
        if (IsUpgraded) card.UpgradeInternal();
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }


}