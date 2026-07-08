using Automaton.AutomatonCode.Encode;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Automaton.AutomatonCode.Powers;

public class FullReleasePower : CustomPowerModel, IAddDumbVariablesToPowerDescription
{

    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    
    
    public override string CustomPackedIconPath => $"{IconName}.tres".DownfallPowerImagePath();
    public override string CustomBigIconPath => $"{IconName}.png".DownfallBigPowerImagePath();
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => _vars;
    private IEnumerable<DynamicVar> _vars = Encodable.All.Select(e => e.FunctionDynamicVar);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => Encodable.All.SelectMany(e => e.DynamicVar(this).BaseValue > 0 ? e.HoverTips(this) : []);

    public void SetDynamicalVars(DynamicVarSet functionCardDynamicVars)
    {
        _dynamicVars = functionCardDynamicVars.Clone(this);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (Owner.Player != player || Owner.CombatState == null) return;
      
        var target = Owner.Player.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
        foreach (var encodable in Encodable.All.Where(e => e is not PowerEncode))
        {
            if (encodable.DynamicVar(this).BaseValue > 0)
                await encodable.OnPlay(this, ctx, target, null);
        }
        Flash();
    }
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        var lines = (from encodable in Encodable.All
            where encodable is not PowerEncode
            where encodable.DynamicVar(this).BaseValue > 0
            select encodable.GetDescription(this).GetFormattedText()).ToList();
        description.Add("effects", string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l))));
    }
}