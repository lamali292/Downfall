using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MirePit : AwakenedCardModel
{
    public MirePit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithPower<TemporaryStrengthDownPower>(6, 2);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var combatStateEnemy in CombatState.Enemies)
            await CommonActions.Apply<TemporaryStrengthDownPower>(combatStateEnemy, this,
                DynamicVars.Power<TemporaryStrengthDownPower>().BaseValue);

        await CommonActions.ApplySelf<DrainedPower>(this, 1);
    }
}