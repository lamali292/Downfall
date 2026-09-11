using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
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
public class Ember : CollectorCardModel
{
    public Ember() : base(-1, CardType.Status, CardRarity.Status, TargetType.Self)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithTip(CardKeyword.Exhaust);
        WithPower<StrengthPower>(1, 1);
        WithVar(new DamageVar(1, DamageProps.cardUnpowered).WithUpgrade(1));
    }
    public override bool HasTurnEndInHandEffect => true;
    public override int MaxUpgradeLevel => 1 + CurrentUpgradeLevel;
    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card,
        bool causedByEthereal)
    {
        if (card != this) return;
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
    
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        var instance = NCombatRoom.Instance;
        instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        await CompatibilityCreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage.IntValue, BlockProps.cardUnpowered, this, null);
    }
}