using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class AncientSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.monsterMove),
        new CardsVar(1)
    ];

    public override SlimeType SlimeType => SlimeType.Specialist;

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<PotencyPower>()
    ];
    
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("ancient"));
        skeleton.SetSlotsToSetupPose();
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingRandomOpponents(CombatState)
            .Execute(ctx);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (!Creature.IsAlive || player != Creature.PetOwner) return count;
        //var original = DynamicVars.Cards.IntValue;
        //var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, original, out _, this);
        return count + DynamicVars.Cards.IntValue;
    }
}