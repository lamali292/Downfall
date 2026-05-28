using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class HyperBeamAutomaton : AutomatonCardModel
{
    public HyperBeamAutomaton() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(18, 4);
        WithPower<VulnerablePower>(1, 1);
        WithTip(AutomatonTip.Stash);
        this.WithTip<Void>();
        WithCards(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.CardAttack(this, cardPlay)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(BeforeDamageAction)
            .Execute(ctx);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await StashCmd.Stash<Void>(Owner, DynamicVars.Cards.IntValue);
    }

    private async Task BeforeDamageAction()
    {
        var enemies = CombatState?.Enemies.Where(e => e.IsAlive).ToList();
        if (enemies != null)
        {
            var vfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
            if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        }

        await Cmd.Wait(0.5f);
        if (enemies != null)
            foreach (var impact in enemies
                         .Select(enemy => NHyperbeamImpactVfx.Create(Owner.Creature, enemy))
                         .OfType<NHyperbeamImpactVfx>())
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
    }
}