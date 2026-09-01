using Automaton.AutomatonCode.Core;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Powers;

public class ItsAFeaturePower : AutomatonPowerModel
{
    public ItsAFeaturePower()
    {
        WithTip<StrengthPower>();
        WithTip<DexterityPower>();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner) return;
        await PowerCmd.Apply<ItsAFeaturePowerStrengthPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Apply<ItsAFeaturePowerDexterityPower>(ctx, Owner, Amount, Owner, null);
    }
}

public class ItsAFeaturePowerStrengthPower : CustomTemporaryPowerModelWrapper<ItsAFeaturePower, StrengthPower>;
public class ItsAFeaturePowerDexterityPower : CustomTemporaryPowerModelWrapper<ItsAFeaturePower, StrengthPower>;