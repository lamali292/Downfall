using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Displace : HexaghostCardModel
{
    public Displace() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(6, 2);
        WithDamage(6, 2);
        WithCards(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = (await DownfallCardCmd.SelectFromHand(ctx, DownfallCardSelectorPrefs.ToTopSelectionPrompt, this))
            .FirstOrDefault();
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}