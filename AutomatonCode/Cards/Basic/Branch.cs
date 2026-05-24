using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Basic;

[Pool(typeof(AutomatonCardPool))]
public class Branch : AutomatonCardModel
{
    public Branch() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithBlock(6, 2);
        WithDamage(7, 2);
        WithTip(AutomatonTip.Encode);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Create two temporary cards representing each branch
        if (CombatState == null) return;
        var attackOption = CombatState.CreateCard<BranchAttack>(cardPlay.Card.Owner);
        var blockOption = CombatState.CreateCard<BranchBlock>(cardPlay.Card.Owner);

        // Copy upgraded values across
        attackOption.DynamicVars.Damage.BaseValue = DynamicVars.Damage.BaseValue;
        blockOption.DynamicVars.Block.BaseValue = DynamicVars.Block.BaseValue;

        var chosen = await CardSelectCmd.FromChooseACardScreen(
            ctx,
            [attackOption, blockOption],
            Owner
        );

        if (chosen == attackOption)
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
            await AutomatonCmd.EncodeCard(blockOption, ctx);
        }
        else
        {
            await CommonActions.CardBlock(this, cardPlay);
            await AutomatonCmd.EncodeCard(attackOption, ctx);
        }
    }
}