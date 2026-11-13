using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Recollect : SlimeBossCardModel
{
    // Set on play, consumed by the next BeforeHandDraw. A plain field is enough: combat cards are clones, so
    // nothing leaks into the next combat.
    private bool _returnNextTurn;

    public Recollect() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        _returnNextTurn = true;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || !_returnNextTurn) return;
        _returnNextTurn = false;
        if (Pile == null || Pile.Type == PileType.Hand) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }
}
