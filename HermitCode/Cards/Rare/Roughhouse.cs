using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Rare;

public class Roughhouse : HermitCardModel, IHasDeadOnEffect
{
    private AttackCommand? _result;
    public Roughhouse() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(22, 6);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();
    public override bool GainsBlock => true;
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        _result = await CommonActions.CardAttack(this, play).WithHermitBluntHeavyHitFx()
            .Execute(ctx);
    }
    
    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        if (_result == null) return;
        var unblockedDamage = _result.Results.SelectMany(e => e).Sum(e => e.TotalDamage + e.OverkillDamage);
        var hasSnipe = Owner.Creature.HasPower<SnipePower>() ? 2 : 1;
        for (var i = 0; i < hasSnipe; i++)
            await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, BlockProps.card, play);
        _result = null;
    }


   
}