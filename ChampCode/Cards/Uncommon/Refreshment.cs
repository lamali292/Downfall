using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Refreshment : ChampCardModel
{
    public Refreshment() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithEnergy(2, 1);
        WithCards(3, 1);
        WithTip(CardKeyword.Exhaust);
        WithBerserkerCombo();
        WithDefensiveCombo();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.BerserkerCombo(cardPlay, async () =>
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            await CardCmdCompatibility.Exhaust(ctx, this);
        });
        await ChampCmd.DefensiveCombo(cardPlay, () => CommonActions.Draw(this, ctx));
    }
}