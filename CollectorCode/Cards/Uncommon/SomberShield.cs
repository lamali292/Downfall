using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class SomberShield : CollectorCardModel
{
    public SomberShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CollectorTip.Pyred);
        WithBlock(4, 3);
        WithPower<CopyNextTurnPower>(1, false);
    }
    
    protected override Artist Artist => Artist.Get<Opal>();
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var pyredCard = await PyreCmd.Pyre(ctx, this);
        if (pyredCard == null) return;

        await CommonActions.CardBlock(this, cardPlay);
        var a = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this);
        a?.Card = pyredCard.CreateClone();
    }


}