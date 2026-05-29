using BaseLib.Utils;
using Guardian.GuardianCode.Cards.Uncommon;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SentryWave : GuardianCardModel
{
    public SentryWave() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(1);
        this.WithBrace(0, 2);
        WithUpgradingCardTip<SentryBlast>();
        WithTip(GuardianTip.Stasis);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        if (IsUpgraded) await GuardianCmd.Brace(ctx, this);
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var card = CombatState!.CreateCard<SentryBlast>(Owner);
        if (IsUpgraded) CardCmd.Upgrade(card);
        await CardPileCmd.Add(card, PileType.Hand);
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}