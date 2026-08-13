using BaseLib.Abstracts;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class MimicryPower : SneckoPowerModel
{
    public MimicryPower()
    {
        WithTip<StrengthPower>();
        WithTip(DownfallTip.Offclass);
    }


    // Mimicry can be offclass for other chars. and gives strength directly then
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !DownfallCmd.IsOffclass(cardPlay.Card)) return;
        await PowerCmd.Apply<MimicryPowerPower>(ctx, Owner, Amount, Owner, null);
    }
}


public class MimicryPowerPower : CustomTemporaryPowerModelWrapper<MimicryPower, StrengthPower>;