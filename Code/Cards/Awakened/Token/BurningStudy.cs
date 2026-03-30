using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class BurningStudy : AwakenedCardModel, ISpell
{
    public BurningStudy() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithPower<StrengthPower>(1);
        WithPower<WeakPower>(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.ApplySelf<StrengthPower>(this, DynamicVars.Power<StrengthPower>().BaseValue);
        foreach (var combatStateEnemy in CombatState.Enemies)
            await CommonActions.Apply<WeakPower>(combatStateEnemy, this, DynamicVars.Power<WeakPower>().BaseValue);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<StrengthPower>().UpgradeValueBy(1);
        DynamicVars.Power<WeakPower>().UpgradeValueBy(1);
    }
}