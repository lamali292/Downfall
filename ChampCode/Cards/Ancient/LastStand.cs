using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Ancient;

[Pool(typeof(ChampCardPool))]
public class LastStand : ChampCardModel
{
    public LastStand() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithPower<StrengthPower>(6);
        WithTip(typeof(WeakPower));
        WithTip(typeof(VulnerablePower));
        WithTip(typeof(FrailPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await PowerCmd.Remove<WeakPower>(Owner.Creature);
        await PowerCmd.Remove<VulnerablePower>(Owner.Creature);
        await PowerCmd.Remove<FrailPower>(Owner.Creature);
    }
}