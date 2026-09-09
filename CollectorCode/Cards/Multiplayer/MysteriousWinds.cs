using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Multiplayer;

[Pool(typeof(CollectorCardPool))]
public class MysteriousWinds : CollectorCardModel, IUsesPyredCards
{
    public MysteriousWinds() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithPower<CopyNextTurnPower>(1, false);
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var powers = await CommonActions.Apply<CopyNextTurnPower>(ctx, this, cardPlay);
        var pyredCard = PyredCards.FirstOrDefault();
        if (pyredCard == null) return;
        foreach (var copyNextTurnPower in powers)
        {
            var card = pyredCard.CreateClone();
            if (IsUpgraded && card.IsUpgradable) CardCmd.Upgrade(card);
            copyNextTurnPower.Card = card;
        }
    }

    public IEnumerable<CardModel> PyredCards { get; set; } = [];
}