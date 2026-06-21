using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Clone : GuardianCardModel
{
    public Clone() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        this.WithAccelerate(0, 1);
        WithTip(GuardianTip.Stasis);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.ApplySelectionPrompt, this))
            .FirstOrDefault();
        if (card == null) return;
        var clone = card.CreateClone();
        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, Owner);
        await GuardianCmd.PutIntoStasis(clone, ctx, this);
        if (IsUpgraded)
            await GuardianCmd.Accelerate(ctx, this);
    }
}