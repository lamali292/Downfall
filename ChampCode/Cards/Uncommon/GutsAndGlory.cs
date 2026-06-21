using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class GutsAndGlory : ChampCardModel
{
    public GutsAndGlory() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithGlory(4, 2);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GloryPower>(ctx, this);
    }
}