using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class SomberShield : CollectorCardModel, IUsesPyredCards
{
    public SomberShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithBlock(6, 3);
        WithPower<CopyNextTurnPower>(1, false);
    }
    public IEnumerable<CardModel> PyredCards { get; set; }

    protected override Artist Artist => Artist.Get<Opal>();
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var a = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this);
        var pyredCard = PyredCards.FirstOrDefault();
        if (a == null || pyredCard == null) return;
        a.Card = pyredCard.CreateClone();
    }


}