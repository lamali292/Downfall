using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class GoodCleanFight : ChampCardModel
{
    public GoodCleanFight() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
        WithPower<DexterityPower>(2, 1);
        WithBerserkerCombo();
        WithDefensiveCombo();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.BerserkerCombo(cardPlay, () => CommonActions.ApplySelf<StrengthPower>(ctx, this));
        await ChampCmd.DefensiveCombo(cardPlay, () => CommonActions.ApplySelf<DexterityPower>(ctx, this));
    }
}