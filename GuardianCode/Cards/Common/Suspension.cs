using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class Suspension : GuardianCardModel, IGemSocketCard
{
    public Suspension() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithTip(GuardianTip.Stasis);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    public int GemSlots => 1;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);

        var card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.StasisSelectionPrompt, this))
            .FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}