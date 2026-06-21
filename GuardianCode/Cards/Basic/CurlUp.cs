using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class CurlUp : GuardianCardModel
{
    public CurlUp() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        this.WithBrace(10, 2);
        WithTip(GuardianTip.Stasis);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        if (GuardianCmd.CanPutIntoStasis(Owner))
        {
            CardModel? card;
            if (IsUpgraded)
                card =
                    (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.StasisSelectionPrompt, this))
                    .FirstOrDefault();
            else
                card = CombatState.RunState.Rng.CombatCardSelection.NextItem(Owner.GetHand(e => e != this));

            if (card != null) await GuardianCmd.PutIntoStasis(card, ctx, this);
        }

        await GuardianCmd.Brace(ctx, this);
    }
}