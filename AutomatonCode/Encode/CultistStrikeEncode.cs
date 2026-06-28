using Automaton.AutomatonCode.Cards.Rare;
using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Encode;

public class CultistStrikeEncode : EncodeModifier
{
    private const int Base = 6;
    private int _increased;
    private int _upgradeBonus; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(Base, ValueProp.Move)];

    private int CurrentDamage => Base + _increased + _upgradeBonus;
    
    public override void StoreSaveData(ModifierSave save)
    {
        save.IntProperties["Increased"] = _increased;
        save.IntProperties["UpgradeBonus"] = _upgradeBonus;
    }

    public override void LoadSaveData(ModifierSave save)
    {
        save.IntProperties.TryGetValue("Increased", out _increased);
        save.IntProperties.TryGetValue("UpgradeBonus", out _upgradeBonus);
        SyncTooltip();
    }

    public override void OnInitialApplication() => SyncTooltip();

    private void SyncTooltip() => DynamicVars.Damage.BaseValue = CurrentDamage;
    
    public void Buff(int extra)
    {
        _increased += extra;
        SyncTooltip();
    }

    public override void OnUpgrade()
    {
        _upgradeBonus += 2; 
        SyncTooltip();
    }

    public override void OnDowngrade()
    {
        _upgradeBonus -= 2; 
        SyncTooltip();
    }

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || Owner == null) return;
        await DamageCmd.Attack(CurrentDamage)
            .FromCard(Owner)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}