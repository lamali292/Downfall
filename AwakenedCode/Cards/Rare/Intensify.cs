using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Intensify : AwakenedCardModel
{
    public Intensify() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<IntensifyPower>(1, false);
        this.WithPower<BurnoutPower>(1, false);
        WithConjure();
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await CommonActions.ApplySelf<IntensifyPower>(ctx, this);
        await CommonActions.ApplySelf<BurnoutPower>(ctx, this);
    }
}