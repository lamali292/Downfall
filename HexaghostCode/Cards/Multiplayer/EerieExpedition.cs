using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class EerieExpedition : HexaghostCardModel
{
    public EerieExpedition() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip(HexaghostKeyword.Afterlife);
        WithCostUpgradeBy(-1);
    }


    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var player in CombatState.Players)
        {
            var card = HexaghostCmd.GetAfterlifeCards(player, 1).FirstOrDefault();
            if (card == null) continue;
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }
}