using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class HoneBladePower : ChampPowerModel
{
    private  int Threshold => DynamicVars.Cards.IntValue;

    public HoneBladePower()
    {
        WithEnergy(1);
        WithCards(3);
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount
    {
        get
        {
            var threshold = Threshold;
            if (threshold <= 0) return 0;
            return threshold - GetInternalData<Data>().StrikesPlayed % threshold;
        }
    }
    protected override object InitInternalData() => new Data();

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || !card.Tags.Contains(CardTag.Strike))
            return;

        var data = GetInternalData<Data>();
        data.StrikesPlayed++;

        var triggers = data.StrikesPlayed / Threshold - data.TriggerCount;
        if (triggers > 0)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount * triggers, card.Owner);
            data.TriggerCount += triggers;
        }

        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int StrikesPlayed;
        public int TriggerCount;
    }
}