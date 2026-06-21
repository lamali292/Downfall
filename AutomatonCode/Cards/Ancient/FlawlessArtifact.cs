using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Ancient;

[Pool(typeof(AutomatonCardPool))]
public class FlawlessArtifact : AutomatonCardModel
{
    public FlawlessArtifact() : base(0, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithKeywords(CardKeyword.Innate, CardKeyword.Retain);
        WithUpgradingCardTip<Constructor>();
        WithUpgradingCardTip<Separator>();
        WithUpgradingCardTip<Terminator>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<Constructor>(Owner, PileType.Hand, upgraded: IsUpgraded);
        await DownfallCardCmd.GiveCard<Separator>(Owner, PileType.Hand, upgraded: IsUpgraded);
        await DownfallCardCmd.GiveCard<Terminator>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }
}