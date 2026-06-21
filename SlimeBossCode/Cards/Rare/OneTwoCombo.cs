using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class OneTwoCombo : SlimeBossCardModel, IAfterSplit
{
    public OneTwoCombo() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(4, 3);
        this.WithCommand(1);
    }

    public async Task AfterSplit(Player player, SlimeModel slime)
    {
        if (player != Owner || Pile?.Type != PileType.Discard) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Command(ctx, this);
    }
}