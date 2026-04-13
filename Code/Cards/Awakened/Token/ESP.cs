using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class ESP() : AwakenedCardModel(1, CardType.Skill, CardRarity.Token, TargetType.Self), ISpell
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];

    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, cardPlay.Card.Owner);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}