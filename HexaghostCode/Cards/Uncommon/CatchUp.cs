using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class CatchUp : HexaghostCardModel
{
    public CatchUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithVar("IgniteCount", 2);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars["IgniteCount"].IntValue; i++)
            await HexaghostCmd.IgnitePrevious(ctx, Owner);
    }
}