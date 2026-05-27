using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class SomberShield : CollectorCardModel, IHasPyre
{
    public SomberShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithBlock(6, 3);
        WithPower<CopyNextTurnPower>(1);
    }

    public CardModel? PyredCard { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var a = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this);
        if (a == null || PyredCard == null) return;
        a.Card = PyredCard.CreateClone();
    }
}