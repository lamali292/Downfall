using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Multiplayer;

[Pool(typeof(Core.SlimeBossCardPool))]
public class BattleCry : SlimeBossCardModel
{
    public BattleCry() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<BattleCryStrengthPower>(2, 1, false);
        WithTip<StrengthPower>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<BattleCryStrengthPower>(ctx, Owner.AllTeammates.Select(e => e.Creature), this);
    }
}

public class BattleCryStrengthPower : CustomTemporaryPowerModelWrapper<BattleCry, StrengthPower>;
