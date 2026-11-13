using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class NibbleAndLick : SlimeBossCardModel
{
    public NibbleAndLick() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(3, 1);
        WithBlock(8, 3);
        WithTip<Slimed>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var block = DynamicVars.Block;
        var amount = Hook.ModifyBlock(CombatState!,Owner.Creature, block.BaseValue, block.Props, this, cardPlay, out _);
        await PowerCmd.Apply<BlockNextTurnPower>(ctx, Owner.Creature, amount, Owner.Creature, this);
        await DownfallCardCmd.GiveCard<Slimed>(Owner, PileType.Draw, CardPilePosition.Top);
    }
}
