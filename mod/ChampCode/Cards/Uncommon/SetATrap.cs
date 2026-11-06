using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class SetATrap : ChampCardModel
{
    public SetATrap() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithBlock(8, 2);
        WithPower<WeakPower>(1, 1);
        WithDefensiveCombo();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ChampCmd.DefensiveCombo(cardPlay, async () =>
        {
            if (CombatState == null) return;
            await CommonActions.Apply<WeakPower>(ctx, CombatState.HittableEnemies, this);
        });
    }
}