using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class StokeTheFire : HexaghostCardModel
{
    public StokeTheFire() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var ignitedCount = HexaghostCmd.GetIgnitedCount(Owner);
        if (ignitedCount == 0 || CombatState == null) return;
        var randomHandCards =
            Owner.GetHand().TakeRandom(ignitedCount, CombatState.RunState.Rng.CombatCardSelection);
        foreach (var card in randomHandCards)
            if (card != this && card.IsUpgradable)
                CardCmd.Upgrade(card);
    }
}