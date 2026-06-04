using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Multiplayer;

[Pool(typeof(GuardianCardPool))]
public class SharedFlux : GuardianCardModel
{
    public SharedFlux() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target?.Player is null) return;
        var target = cardPlay.Target.Player;
        if (!GuardianCmd.CanPutIntoStasis(target, Owner)) return;
        var card = (await CardSelectCmd.FromHand(
            ctx,
            target,
            new CardSelectorPrefs(
                DownfallCardSelectorPrefs.StasisSelectionPrompt,
                0,
                1
            ),
            null,
            this
        )).FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}