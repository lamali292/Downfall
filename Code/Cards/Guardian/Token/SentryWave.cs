using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Uncommon;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Guardian.Token;

[Pool(typeof(TokenCardPool))]
public class SentryWave : GuardianCardModel
{
    public SentryWave() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        WithBrace(0, 2);
        WithUpgradedCardTip<SentryBlast>();
        WithTip(DownfallTip.Stasis);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<WeakPower>(this, cardPlay);
        if (IsUpgraded) await GuardianCmd.Brace(this);
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var card = CombatState!.CreateCard(ModelDb.Card<SentryBlast>(), Owner);
        if (IsUpgraded) card.UpgradeInternal();
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}