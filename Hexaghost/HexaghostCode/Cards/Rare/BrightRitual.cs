using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class BrightRitual : HexaghostCardModel
{
    public BrightRitual() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithEnergy(1);
        WithCards(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = await HexaghostCmd.ResetWheel(Owner);
        await PlayerCmd.GainEnergy(amount*DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(ctx, amount*DynamicVars.Cards.BaseValue, Owner);
    }


}