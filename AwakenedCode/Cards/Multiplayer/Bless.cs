using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class Bless : AwakenedCardModel
{
    public Bless() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<StrengthPower>(2, 1);
        WithPower<StrengthPower>("StrengthLoss",2);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, -DynamicVars["StrengthLoss"].BaseValue,
            Owner.Creature, this);
        await CommonActions.Apply<StrengthPower>(ctx, this, cardPlay);
    }
}