using BaseLib.Extensions;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.DynamicVars;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rooms;

namespace Champ.ChampCode.Potions;

[Pool(typeof(ChampPotionPool))]
public class BottledTechnique : ChampPotionModel
{
    public BottledTechnique() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithRepeat(5);
        WithTip(ChampKeyword.TriggerSkillBonus);
        WithTip(ChampTip.Stance);
    }

    public override bool PassesCustomUsabilityCheck
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress || Owner.RunState.CurrentRoom is not CombatRoom)
                return false;
            return Owner.ChampStance() is not ChampNoStance;
        }
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var a = ChampModel.GetStanceModel(Owner);
        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
            await a.SkillBonus(ctx);
    }
}