using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MassFeed : SlimeBossCardModel
{
    public MassFeed() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(10, 2);
        WithVars(new MaxHpVar(2).WithUpgrade(1));
        WithTip(StaticHoverTip.Fatal);
        WithKeyword(CardKeyword.Exhaust);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;

        var fatalEligible = CombatState.HittableEnemies
            .Where(e => e.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
            .ToHashSet();
        var attackCommand = await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_bite", tmpSfx: "blunt_attack.mp3")
            .Execute(ctx);

        var anyFatalKill = attackCommand.Results
            .SelectMany(r => r)
            .Any(r => r.WasTargetKilled && fatalEligible.Contains(r.Receiver));

        if (!anyFatalKill) return;
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.IntValue);
    }
}