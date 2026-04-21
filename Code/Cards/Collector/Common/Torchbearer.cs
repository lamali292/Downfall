using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Torchbearer : CollectorCardModel
{
    public Torchbearer() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVars(new SummonVar(10).WithUpgrade(4));
        WithKeyword(CardKeyword.Exhaust);
        WithTip(DownfallTip.Kindle);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
    }
}