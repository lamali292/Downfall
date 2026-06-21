using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class BronzeArmor : AutomatonCardModel
{
    public BronzeArmor() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(13, 4);
        WithStash(2);
        this.WithTip<Error>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StashCmd.Stash<Error>(Owner, DynamicVars["Stash"].IntValue);
    }
}