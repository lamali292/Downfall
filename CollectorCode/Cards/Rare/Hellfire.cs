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
public class Hellfire : CollectorCardModel, IHasPyre
{
    public Hellfire() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Megapyre);
        WithTip(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
        WithPower<MiasmaPower>(6, 3);
    }

    public CardModel? PyredCard { get; set; }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var list = Owner.Hand.ToList();
        var cardCount = list.Count;
        foreach (var card2 in list)
            PyredCard = card2;//Is this a bad idea?
        await CardCmdCompatibility.Exhaust(ctx, PyredCard!).ConfigureAwait(false);
        for (var i = 0; i < cardCount; i++)
        {
            await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        }
    }
}