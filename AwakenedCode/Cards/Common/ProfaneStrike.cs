using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class ProfaneStrike : AwakenedCardModel
{
    public ProfaneStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(11, 4);
    }

    protected override Artist Artist => Artist.Get<Occultpyromancer>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.ToTopSelectionPrompt, this);
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}