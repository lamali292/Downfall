using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class LingeringShades : HexaghostCardModel
{
    public LingeringShades() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(HexaghostKeyword.Retract);
        WithPower<SoulBurnPower>(14, 4);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        if (Owner.PlayerCombatState == null) return;
        await CardPileCmd.Add(
            Owner.PlayerCombatState.DiscardPile.Cards.Where(c => c.Keywords.Contains(CardKeyword.Ethereal)),
            PileType.Hand);
    }


}