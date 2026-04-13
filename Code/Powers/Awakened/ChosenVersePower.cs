using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class ChosenVersePower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public CardPlay? CardPlay;
    
    public override bool IsInstanced => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Unpowered)
    ];

    public void SetBlock(int block)
    {
        DynamicVars.Block.BaseValue = block;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (cardPlay == CardPlay) return;
        if (cardPlay.Card.Type == CardType.Attack) return;
        await CardPileCmd.Draw(context, 1, cardPlay.Card.Owner);
        await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null);
        Flash();
        await PowerCmd.Decrement(this);
    }

}