using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class Branch() : AutomatonCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new DamageVar(7m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [DownfallKeyword.Encode.ToHoverTip()];

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Create two temporary cards representing each branch
        var attackOption = CombatState!.CreateCard<BranchAttack>(cardPlay.Card.Owner);
        var blockOption = CombatState!.CreateCard<BranchBlock>(cardPlay.Card.Owner);

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
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
                .Targeting(cardPlay.Target).Execute(ctx);
            await AutomatonCmd.EncodeCard(blockOption, ctx, cardPlay);
        }
        else
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            await AutomatonCmd.EncodeCard(attackOption, ctx, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}