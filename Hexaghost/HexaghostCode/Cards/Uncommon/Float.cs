using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Float : HexaghostCardModel
{
    public Float() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(3, 2);
        WithCards(1);
        WithVar("CardsPlayed", 3, 2);
    }

    protected override bool ShouldGlowGoldInternal => CombatManager.Instance.History.Entries
        .OfType<CardPlayFinishedEntry>().Count(e => e.HappenedThisTurn(CombatState)) < DynamicVars["CardsPlayed"].IntValue;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var a = CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>().Count(e => e.HappenedThisTurn(CombatState));
        if (a >= DynamicVars["CardsPlayed"].IntValue)  return;
        await CommonActions.Draw(this, ctx);
        await HexaghostCmd.Advance(ctx, Owner, this);
    }
}