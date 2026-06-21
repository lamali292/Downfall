using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class SentryBlast : GuardianCardModel
{
    public SentryBlast() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithUpgradingCardTip<SentryWave>();
        WithTip(GuardianTip.Stasis);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var card = CombatState!.CreateCard<SentryWave>(Owner);
        if (IsUpgraded) CardCmd.Upgrade(card);
        await CardPileCmd.Add(card, PileType.Hand);
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}