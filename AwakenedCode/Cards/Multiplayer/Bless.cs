using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

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