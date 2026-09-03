using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Hellfire : CollectorCardModel
{
    public Hellfire() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithPower<MiasmaPower>(6, 3);
    }
    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var list = Owner.Hand.ToList();
        var cardCount = list.Count;
        foreach (var card2 in list)
            await CardCmdCompatibility.Exhaust(ctx, card2);
        for (var i = 0; i < cardCount; i++)
        {
            await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        }
    }
}