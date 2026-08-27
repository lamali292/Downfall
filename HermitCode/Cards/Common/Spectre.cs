using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Common;

public class Spectre : HermitCardModel
{
    public Spectre() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(2, 2);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<DawnablesAwakened>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (CombatState == null) return;
        CardModel? card;
        if (IsUpgraded)
            card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.AddEtherealSelectionPrompt,
                    this, c => !c.Keywords.Contains(CardKeyword.Ethereal)))
                .FirstOrDefault();
        else
            card = CombatState.RunState.Rng.CombatCardSelection.NextItem(Owner.Hand.Where(e =>
                e != this && !e.Keywords.Contains(CardKeyword.Ethereal)));
        card?.AddKeyword(CardKeyword.Ethereal);
    }
}