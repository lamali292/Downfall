using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class WaveOfMiasma : AwakenedCardModel
{
    public WaveOfMiasma() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12);
        WithPower<ManaburnPower>(4);
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardBlock(this, cardPlay);
        var currentEnemies = CombatState.Enemies.Where(e => e.IsAlive).ToList(); 
        foreach (var enemy in currentEnemies)
        {
            await CommonActions.Apply<ManaburnPower>(enemy, this, DynamicVars.Power<ManaburnPower>().BaseValue);
        }
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        DynamicVars.Power<ManaburnPower>().UpgradeValueBy(2);
    }
}