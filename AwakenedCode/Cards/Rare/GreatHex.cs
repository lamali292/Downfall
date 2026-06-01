using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class GreatHex : AwakenedCardModel, IChantable
{
    public GreatHex() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        this.WithPower<GreatHexPower>(5, 3, false);
        this.WithTip<ManaburnPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    public bool HasChanted { get; set; } = false;

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GreatHexPower>(ctx, this, cardPlay);
    }
}