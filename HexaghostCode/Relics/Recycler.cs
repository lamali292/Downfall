using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class Recycler : HexaghostRelicModel
{
    public Recycler() : base(RelicRarity.Uncommon)
    {
        WithTip(HexaghostKeyword.Afterlife);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
    }

    private bool UsedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
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
        if (UsedThisCombat || !causedByEthereal || card.Owner != Owner ||
            !card.Keywords.Contains(HexaghostKeyword.Afterlife)) return;
        var clone = card.CreateClone();
        clone.GiveSingleTurnRetain();
        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, Owner);
        UsedThisCombat = true;
        Flash();
        Status = RelicStatus.Normal;
    }
}