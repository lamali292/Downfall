using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Gather : AwakenedCardModel
{
    public Gather() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(3, 3);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, cardPlay);
        await ChantCmd.Chant(cardPlay, async () =>
        {
            var selected = await CommonActions.SelectSingleCard(this, DownfallCardSelectorPrefs.ToHandSelectionPrompt, ctx,
                PileType.Discard);
            if (selected == null) return;
            await CardPileCmd.Add(selected, PileType.Hand);
        });
    }
}