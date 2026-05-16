using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class Recycler() : HexaghostRelicModel(RelicRarity.Uncommon)
{
    private bool _usedThisCombat;

    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }


    public override Task BeforeCombatStart()
    {
        Status = RelicStatus.Active;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        UsedThisCombat = false;
        return Task.CompletedTask;
    }


    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (UsedThisCombat || card.Owner != Owner || !card.Keywords.Contains(CardKeyword.Ethereal) ||
            card.Type is CardType.Curse or CardType.Status) return;
        await CardPileCmd.Add(card.CreateClone(), PileType.Hand);
        UsedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;
    }
}