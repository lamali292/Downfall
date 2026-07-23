using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class ThrowingCardsPower : SneckoPowerModel
{
    public ThrowingCardsPower()
    {
        WithDamage(6);
        WithCards(1);
        WithTip(SneckoTip.Offclass);
    }
    
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        var card = cardPlay.Card;
        if (!SneckoCmd.IsOffclass(card)) return;
        var a = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        Flash();
        await PowerCmd.Decrement(this);
        if (a != null)
            await DownfallCreatureCmd.Damage(ctx, a, DynamicVars.Damage.BaseValue, ValueProp.Unpowered, 
                Owner, null, null);
        await MyCommonActions.Draw( this, ctx);
    }
}