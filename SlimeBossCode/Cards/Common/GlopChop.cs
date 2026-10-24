using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class GlopChop : SlimeBossCardModel
{
    private const string DecreaseKey = "Decrease";

    public GlopChop() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(16);
        WithVar(new DynamicVar(DecreaseKey, 4).WithUpgrade(-2));
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        // Like Rampage: the combat copy's damage is permanently changed for the rest of the combat.
        DynamicVars.Damage.BaseValue = Math.Max(0, DynamicVars.Damage.BaseValue - DynamicVars[DecreaseKey].BaseValue);
    }
}
