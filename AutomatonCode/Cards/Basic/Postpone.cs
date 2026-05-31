using Automaton.AutomatonCode.Cards.Ancient;
using Automaton.AutomatonCode.Core;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Basic;

[Pool(typeof(AutomatonCardPool))]
public class Postpone : AutomatonCardModel, ITranscendenceCard
{
    public Postpone() : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(10, 4);
        WithStash(1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Hook>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StashCmd.StashFromHand(this, ctx);
    }
}