using BaseLib.Abstracts;
using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Common;

[Pool(typeof(GremlinsCardPool))]
public class Irritability : GremlinsCardModel
{
    public Irritability() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 2);
        WithPower<IrritabilityPower>(3, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<IrritabilityPower>(ctx, this);
        await GremlinsCmd.SwapToType<MadGremlin>(ctx, Owner);
    }
}

public class IrritabilityPower : CustomTemporaryPowerModelWrapper<Irritability, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}