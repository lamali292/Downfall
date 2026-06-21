using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Common;

public class Spectre : HermitCardModel
{
    public Spectre() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        WithTip(CardKeyword.Ethereal);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (CombatState == null) return;
        CardModel? card;
        if (IsUpgraded)
            card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.AddEtherealSelectionPrompt,
                    this))
                .FirstOrDefault();
        else
            card = CombatState.RunState.Rng.CombatCardSelection.NextItem(Owner.GetHand(e => e != this));
        card?.AddKeyword(CardKeyword.Ethereal);
    }
}