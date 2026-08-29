using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Sizzle : HexaghostCardModel
{
    public Sizzle() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<SizzlePower>(1, false);
        WithBlock(13, 4);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist? Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<SizzlePower>(ctx, this);
    }
}