using BaseLib.Extensions;
using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Incorporeal : HexaghostCardModel
{
    public Incorporeal() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVar(new DamageVar(6, ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered).WithUpgrade(-3));
        WithPower<IntangiblePower>(1);
        WithKeywords(CardKeyword.Exhaust, HexaghostKeyword.Retract);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(Owner.Creature).Execute(ctx);
        await CommonActions.ApplySelf<IntangiblePower>(ctx, this);
    }


}