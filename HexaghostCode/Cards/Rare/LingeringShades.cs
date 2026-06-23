using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class LingeringShades : HexaghostCardModel
{
    public LingeringShades() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(HexaghostKeyword.Retract);
        WithPower<SoulBurnPower>(15, 4);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
        await CardPileCmd.Add(
            Owner.GetDiscard().Where(c => c.Keywords.Contains(CardKeyword.Ethereal)),
            PileType.Hand);
    }
}