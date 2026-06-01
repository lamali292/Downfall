using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Undervolt : AutomatonCardModel
{
    public Undervolt() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<StrengthPower>(-2, -1);
        WithVar("StrengthLoss", 2, 1);
        this.WithTip<Burn>();
        WithCards(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<StrengthPower>(ctx, this, cardPlay);
        await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, DynamicVars.Cards.IntValue);
    }
}