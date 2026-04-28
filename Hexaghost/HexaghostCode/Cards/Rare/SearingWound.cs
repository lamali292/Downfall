using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class SearingWound : HexaghostCardModel
{
    public SearingWound() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            var amount = enemy.GetPowerAmount<SoulBurnPower>();
            await CreatureCmd.Damage(ctx, enemy, amount,
                ValueProp.Move | ValueProp.Unpowered | ValueProp.Unblockable, 
                Owner.Creature, this);
        }
    }
}