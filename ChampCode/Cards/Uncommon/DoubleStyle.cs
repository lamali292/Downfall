using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class DoubleStyle : ChampCardModel
{
    public DoubleStyle() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<DefensiveStylePower>(1, 1, false);
        this.WithPower<BerserkerStylePower>(1, 1, false);
        this.WithTip<VigorPower>();
        this.WithTip<CounterPower>();
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DefensiveStylePower>(ctx, this);
        await CommonActions.ApplySelf<BerserkerStylePower>(ctx, this);
    }
}