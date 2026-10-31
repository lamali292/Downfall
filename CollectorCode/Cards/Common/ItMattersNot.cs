using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class ItMattersNot : CollectorCardModel
{
    public ItMattersNot() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithBlock(10, 4);
        WithCardTip<Ember>();
        WithUpgradedCardTip<Ember>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private int _passCost = 0;
    private CardModel? PyredCard { get; set; }
    
    private void Upgrade(Ember obj) => Upgrade(obj, _passCost);

    private static void Upgrade(Ember obj, int times)
    {
        for (var i = 0; i < times; i++)
        {
            CardCmd.Upgrade(obj);
        }
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        if (PyredCard != null && PyredCard.EnergyCost.GetAmountToSpend() > 0)
        {
            _passCost = PyredCard.EnergyCost.GetAmountToSpend();
            await DownfallCardCmd.GiveCard<Ember>(cardPlay.Player, PileType.Hand, action: Upgrade);
        }
        else
        {
            await DownfallCardCmd.GiveCard<Ember>(cardPlay.Player, PileType.Hand);
        }
        await CommonActions.CardBlock(this, cardPlay);
    }
}