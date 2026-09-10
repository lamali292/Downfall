using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Empower : CollectorCardModel
{
    public Empower() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<EmpowerPower>(2, false);
        WithCardTip<Ember>();
        WithCardTip<Ember>(WithPreviewModifiers);
    }

    private void WithPreviewModifiers(Ember ember, CardModel cardModel)
    {
        var x = ResolveEnergyXValue();
        var val = cardModel is { IsMutable: true, _owner: not null }
            ? cardModel.Owner.PlayerCombatState?.Energy ?? 0+x : 1+x;
        if (cardModel.IsUpgraded) val += 1;
        WithModifiers(ember, val);
    }
    
    private static void WithModifiers(Ember ember, int ups)
    {
        DownfallCardCmd.ForceUpgrade(ember, ups);
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        if (IsUpgraded) amount++;
        var a = await CommonActions.ApplySelf<EmpowerPower>(ctx, this);
        a?.SetCards(amount);
    }
}