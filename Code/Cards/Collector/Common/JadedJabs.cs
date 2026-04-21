using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class JadedJabs : CollectorCardModel
{
    public JadedJabs() : base(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPyre();
        WithDamage(15, 2);
        WithVar("JadedJabs", 1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var cost = PyredCard!.EnergyCost.GetWithModifiers(CostModifiers.All);
        var jadedJabs = DynamicVars["JadedJabs"].IntValue;
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, jadedJabs + cost);
    }
}