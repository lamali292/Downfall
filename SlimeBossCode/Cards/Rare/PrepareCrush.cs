using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class PrepareCrush : SlimeBossCardModel
{
    public PrepareCrush() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithUpgradingCardTip<SlimeCrush>();
        this.WithTip<StrengthPower>();
        WithKeyword(CardKeyword.Exhaust);
        WithEnergy(3, 1);
        this.WithPower<StrengthNextTurnPower>(3, 2, false);
        this.WithPower<EnergyNextTurnPower>(3, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this, 1);
        var card = CombatState?.CreateCard<SlimeCrush>(Owner);
        if (card == null || a == null) return;
        if (IsUpgraded) card.UpgradeInternal();
        a.Card = card;
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
        await CommonActions.ApplySelf<StrengthNextTurnPower>(ctx, this);
    }
}