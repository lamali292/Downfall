using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
public class Ember : CollectorCardModel, IStackingUpgradeCard
{
    private bool wasPlayedLast = false;
    
    public Ember() : base(-1, CardType.Status, CardRarity.Status, TargetType.Self)
    {
        WithKeyword(CardKeyword.Unplayable);
        WithTip(CardKeyword.Exhaust);
        WithPower<StrengthPower>(1, 1);
        WithVar(new DamageVar(1, DamageProps.cardUnpowered));
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
        await CompatibilityCreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.Damage.IntValue, DamageProps.cardUnpowered, this, null);
        wasPlayedLast = true;
    }

    public override async Task AfterCardChangedPilesLate(//Late to avoid as much visual jank as possible
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy)
    {
        if (wasPlayedLast)
        {
            //I WAIT!
            await Cmd.Wait(0.25f);
            await Cmd.Wait(0.20f);
            await Cmd.Wait(0.15f);
            await Cmd.Wait(0.10f);
            await CardPileCmd.Add(this, PileType.Hand);
            wasPlayedLast = false;
        }
    }
}