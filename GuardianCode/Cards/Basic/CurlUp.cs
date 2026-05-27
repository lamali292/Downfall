using BaseLib.Utils;
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


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
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
                card = Owner.GetHand(e => e != this)
                    .TakeRandom(1, CombatState.RunState.Rng.CombatCardSelection).FirstOrDefault();

            if (card == null) return;
            await GuardianCmd.PutIntoStasis(card, ctx, this);
        }

        await GuardianCmd.Brace(ctx, this);
    }
}