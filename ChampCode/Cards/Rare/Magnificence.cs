using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class Magnificence : ChampCardModel
{
    public Magnificence() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<MagnificencePower>(3, 1, false);
        this.WithTip<GloryPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MagnificencePower>(ctx, this);
    }
}