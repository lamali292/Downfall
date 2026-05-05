using BaseLib.Utils;
using Downfall.DownfallCode.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Serpentscale : SneckoCardModel
{
    public Serpentscale() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9);
        WithOverflow();
        WithPower<PlatedArmorPower>(3, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (SneckoCmd.OverflowActive(Owner)) return;
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this, 1);
        
      
    }

    protected override Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
     => CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    
}