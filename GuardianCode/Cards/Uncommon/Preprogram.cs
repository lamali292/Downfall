using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Preprogram : GuardianCardModel, IGemSocketCard
{
    public Preprogram() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(5, 3);
        WithTip(GuardianTip.Stasis);
    }

    public int GemSlots => 1;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!GuardianCmd.CanPutIntoStasis(Owner)) return;
        var cards = Owner.GetDraw().Take(DynamicVars.Cards.IntValue).ToList();
        var card = (await DownfallCardCmd.SelectFromCards(ctx, cards, DownfallCardSelectorPrefs.StasisSelectionPrompt,
            this)).FirstOrDefault();
        if (card == null) return;
        await GuardianCmd.PutIntoStasis(card, ctx, this);
    }
}