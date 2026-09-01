using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ProtectiveAura : ChampCardModel
{
    public ProtectiveAura() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<ProtectiveAuraPower>(1, false);
        WithCards(4);
        WithEnergy(1);
        WithCostUpgradeBy(-1);
        WithTags(CardTag.Strike);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ProtectiveAuraPower>(ctx, this);
    }
}