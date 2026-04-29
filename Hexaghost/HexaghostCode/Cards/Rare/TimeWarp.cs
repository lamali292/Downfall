using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class TimeWarp : HexaghostCardModel, IWheelMoved
{
    public TimeWarp() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithTip(HexaghostKeyword.Advance);
        WithTip(HexaghostKeyword.Retract);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this,  cardPlay).Execute(ctx);
    }


    public async Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (Pile == null || player != Owner || Pile.Type != PileType.Discard) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    public async Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source, GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        if (Pile == null || player != Owner || Pile.Type != PileType.Discard) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }
}