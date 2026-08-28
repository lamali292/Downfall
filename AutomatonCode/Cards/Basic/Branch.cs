using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Create two temporary cards representing each branch
        if (CombatState == null) return;
        var attackOption = ModelDb.Card<BranchAttack>().ToMutable();
        var blockOption = ModelDb.Card<BranchBlock>().ToMutable();
        attackOption.Owner = Owner;
        blockOption.Owner = Owner;
        Action(attackOption);
        Action(blockOption);

        var chosen = await CardSelectCmd.FromChooseACardScreen(
            ctx,
            [attackOption, blockOption],
            Owner
        );

        if (chosen == attackOption)
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
            await AutomatonCmd.EncodeCard<BranchBlock>(Owner, ctx, Action);
        }
        else
        {
            await CommonActions.CardBlock(this, cardPlay);
            await AutomatonCmd.EncodeCard<BranchAttack>(Owner, ctx, Action);
        }
    }

    private void Action(CardModel card)
    {
        if (IsUpgraded)
            card.UpgradeInternal();
        var a = (EnchantmentModel?)Enchantment?.MutableClone();
        if (a == null) return;
        if (a.CanEnchant(card)) 
            card.EnchantInternal(a, a.Amount);
    }
}