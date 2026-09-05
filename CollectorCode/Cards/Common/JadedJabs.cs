using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class JadedJabs : CollectorCardModel, IUsesPyredCards
{
    public JadedJabs() : base(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithDamage(15, 2);
        WithVar("JadedJabs", 1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    public IEnumerable<CardModel> PyredCards { get; set; }
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var cost = PyredCards.FirstOrDefault()?.EnergyCost.GetWithModifiers(CostModifiers.All) ?? 0;
        var jadedJabs = DynamicVars["JadedJabs"].IntValue;
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, jadedJabs + cost);
    }


}