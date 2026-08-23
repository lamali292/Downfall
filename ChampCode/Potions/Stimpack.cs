using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Potions;

[Pool(typeof(ChampPotionPool))]
public class Stimpack : ChampPotionModel
{
    public Stimpack() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTips(e => [ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip]);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return ChampCmd.EnterUltimateStance(ctx, Owner, this);
    }
}