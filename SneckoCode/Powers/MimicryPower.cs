using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class MimicryPower : SneckoPowerModel
{
    public MimicryPower()
    {
        WithTip<StrengthPower>();
        WithTip(SneckoTip.Offclass);
    }


    // Mimicry can be offclass for other chars. and gives strength directly then
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !SneckoCmd.IsOffclass(cardPlay.Card)) return;
        await PowerCmd.Apply<MimicryPowerPower>(ctx, Owner, Amount, Owner, null);
    }
}


public class MimicryPowerPower : CustomTemporaryPowerModelWrapper<MimicryPower, StrengthPower>;