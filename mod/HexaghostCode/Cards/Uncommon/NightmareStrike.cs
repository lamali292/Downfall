using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Cards.Token;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class NightmareStrike : HexaghostCardModel, IHasAfterlifeEffect
{
    public NightmareStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithUpgradingCardTip<ShadowStrike>();
        WithDamage(4, 2);
        WithTags(CardTag.Strike);
        WithAfterlife();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthereal)
    {
        await DownfallCardCmd.GiveCard<ShadowStrike>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await AfterlifeEffect(ctx, cardPlay, false, false);
    }
}