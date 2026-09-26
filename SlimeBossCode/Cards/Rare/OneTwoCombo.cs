using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class OneTwoCombo : SlimeBossCardModel, IAfterSplit, IAfterCommand
{
    public OneTwoCombo() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
    }

    private async Task ReturnToHand(Player player)
    {
        if (player != Owner || Pile == null || Pile.Type == PileType.Hand) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    public Task AfterSplit(PlayerChoiceContext ctx, Player player, SlimeModel slime)
    {
        return ReturnToHand(player);
    }

    public Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        return ReturnToHand(player);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
