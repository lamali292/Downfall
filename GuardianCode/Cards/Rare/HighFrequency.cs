using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class HighFrequency : GuardianCardModel
{
    public HighFrequency() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip(GuardianTip.Stasis);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();
    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.StasisSelectionPrompt, this))
            .FirstOrDefault();
        if (card == null) return;

        while (GuardianCmd.CanPutIntoStasis(Owner, silent: true))
        {
            var a = card.CreateClone();
            await CardPileCmd.AddGeneratedCardToCombat(a, PileType.Play, Owner);
            await GuardianCmd.PutIntoStasis(a, ctx, this, true);
        }

        await CardCmdCompatibility.Exhaust(ctx, card);
    }
}