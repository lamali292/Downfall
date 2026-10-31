using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(StatusCardPool))]
public class Ember : CollectorCardModel, IStackingUpgradeCard, IReturnsToHandAfterTurnEnd
{
    
    public Ember() : base(1, CardType.Status, CardRarity.Status, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain);
        WithTip(CardKeyword.Exhaust);
        WithPower<StrengthPower>(1, 1);
        WithVar(new DamageVar(2, DamageProps.cardUnpowered).WithUpgrade(2));
    }
    public override bool HasTurnEndInHandEffect => true;
    public override int MaxUpgradeLevel => 1 + CurrentUpgradeLevel;
    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            if (card != this) return;
            await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        }
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        // idk if this early return fixes the mp bug, hopefully it will
        if (CombatState == null || !Owner.Creature.IsAlive) return;
        var instance = NCombatRoom.Instance;
        var playCount = await GeneratePlayCount(CombatState!, null);
        for (var i = 0; i < playCount; ++i)
        {
            instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            await CompatibilityCreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage.IntValue,
                DamageProps.cardUnpowered, this, null);
        }
    }
}