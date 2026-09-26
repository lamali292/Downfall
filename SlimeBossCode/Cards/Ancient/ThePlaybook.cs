using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Ancient;

[Pool(typeof(SlimeBossCardPool))]
public class ThePlaybook : SlimeBossCardModel
{
    public ThePlaybook() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithPower<ThePlaybookPower>(3, false);
        WithEnergy(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<ThePlaybookPower>(ctx, this))?.AddEnergy(DynamicVars.Energy.IntValue);
    }
}
