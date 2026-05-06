using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SmartFormat.Core.Extensions;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Powers;

public class TyphoonFangPower : SneckoPowerModel, IAfterOverflowEffect
{

    public TyphoonFangPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new CardDynamicVar());
        WithTips(power => 
            power is TyphoonFangPower { Dupe: not null } fang ?
                [new CardHoverTip(fang.Dupe)] : []
            );
    }

    public override bool IsInstanced => true;

    private CardModel? Dupe { get; set; }
    private CardModel? Source { get; set; }
    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        if (card.Owner.Creature != Owner || Source == cardPlay.Card || Source == card || Dupe == null || cardPlay.IsAutoPlay) return;
        var enemy = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (enemy == null)  return;
        Flash();
        await CardCmd.AutoPlay(ctx, Dupe, enemy);
    }

    public void SetCard(CardModel card)
    {
        Dupe = card.CreateDupe();
        Source = card;
    }
    
    private class CardDynamicVar() : DynamicVar("card", 0)
    {

        public override string ToString()
        {
            return _owner is TyphoonFangPower power ? power.Dupe?.Title ?? "" : "";
        }
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
            return;
        await PowerCmd.Remove(this);
    }

}