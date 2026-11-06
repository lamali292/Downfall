using BaseLib.Extensions;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class SkillfulDodge : ChampCardModel
{
    public SkillfulDodge() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(4, 1);
        WithPower<CounterPower>(4, 1);
        WithVar("Increase", 4, 1);
        WithDefensiveCombo();
    }

    protected override Artist Artist => Artist.Get<Magerblutooth>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
        await ChampCmd.DefensiveCombo(cardPlay, () =>
        {
            DynamicVars.Block.UpgradeValueBy(DynamicVars["Increase"].IntValue);
            DynamicVars.Power<CounterPower>().UpgradeValueBy(DynamicVars["Increase"].IntValue);
            return Task.CompletedTask;
        });
    }
}