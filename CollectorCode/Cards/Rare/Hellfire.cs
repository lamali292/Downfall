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
public class Hellfire : CollectorCardModel, IUsesPyredCards
{
    public Hellfire() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Megapyre);
        WithTip(CollectorTip.Pyred);
        WithKeyword(CardKeyword.Exhaust);
        WithPower<MiasmaPower>(6, 3);
    }

    public IEnumerable<CardModel> PyredCards { get; set; } = [];
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var cardCount = PyredCards.Count();
        for (var i = 0; i < cardCount; i++)
        {
            await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        }
    }


}