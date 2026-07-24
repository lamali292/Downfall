using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class NaughtySpirit : HexaghostCardModel, IModifyCardPlayResultLocation
{
    //todo iternal glow check AND if it's on a crushing with one token, or an infernal where energy on this card equals # of required tokens
    public NaughtySpirit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<SoulBurnPower>(3, 2);
        WithTip(HexaghostKeyword.Retract);
    }


    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (this != card || !HexaghostCmd.IsIgnited(card.Owner)) return cardLocation;

        return new CardLocationCompatiblity(card.Owner, PileType.Hand, CardPilePosition.Bottom);
    }


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.ResultPile != PileType.Hand || this != cardPlay.Card) return;
        await HexaghostCmd.Retract(ctx, Owner, this);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}