using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Multiplayer;

[Pool(typeof(SneckoCardPool))]
public class DiceCase : SneckoCardModel
{
    public DiceCase() : base(3, CardType.Power, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithPower<DiceCasePower>(1, false);
        WithTip<SoulRoll>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = ModelDb.Power<DiceCasePower>().ToMutable();
        if (power is DiceCasePower dc) dc.TargetCreature = cardPlay.Target;
        await PowerCmd.Apply(ctx, power, Owner.Creature, DynamicVars.Power<DiceCasePower>().BaseValue, Owner.Creature,
            this);
    }
}