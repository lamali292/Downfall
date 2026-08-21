using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class ChampionsCrown : ChampRelicModel
{

    public ChampionsCrown() : base(RelicRarity.Starter)
    {
        WithTip(ChampTip.Stance);
        WithTip(ChampKeyword.TriggerSkillBonus);
        WithCards(1);
    }
    
    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<VictoriousCrown>();
    }


    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        Flash();
        await ChampCmd.EnterDifferentStance(ctx, player);
        var stance = Owner.ChampStance;
        await stance.SkillBonus(ctx);
        await MyCommonActions.Draw(this, ctx);
    }
}