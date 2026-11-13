using Automaton.AutomatonCode.Core;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        if (card.Owner.Creature != Owner || card.Type is not (CardType.Curse or CardType.Status)) return;
        await PowerCmd.Apply<ItsAFeatureStrengthPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Apply<ItsAFeatureDexterityPower>(ctx, Owner, Amount, Owner, null);
    }
}

public class ItsAFeatureStrengthPower : CustomTemporaryPowerModelWrapper<ItsAFeaturePower, StrengthPower>;
public class ItsAFeatureDexterityPower : CustomTemporaryPowerModelWrapper<ItsAFeaturePower, DexterityPower>;