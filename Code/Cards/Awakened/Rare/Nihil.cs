using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Nihil : AwakenedCardModel, IChantable
{
    public Nihil() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(13);
        WithTip(DownfallKeyword.Chant);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<ManaburnPower>(cardPlay.Target, this, DynamicVars.Power<ManaburnPower>().BaseValue);
    }
    
    public async Task OnChant(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        foreach (var combatStateEnemy in CombatState.Enemies)
        {
            var a = combatStateEnemy.GetPowerAmount<ManaburnPower>();
            if (a <= 0) continue;
            await CreatureCmd.Damage(
                ctx,
                combatStateEnemy,
                a,
                ValueProp.Unpowered | ValueProp.Unblockable,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<ManaburnPower>().UpgradeValueBy(3);
    }
}