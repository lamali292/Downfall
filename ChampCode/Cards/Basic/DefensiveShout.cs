using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Basic;

[Pool(typeof(ChampCardPool))]
public class DefensiveShout : ChampCardModel
{
    public DefensiveShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<CounterPower>(3, 3);
        this.WithEnterDefensive();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
    }
}