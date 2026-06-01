using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class UltimateStance : ChampCardModel
{
    public UltimateStance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        this.WithGlory(10);
        WithCostUpgradeBy(-1);
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<GloryPower>(ctx, this);
    }
}