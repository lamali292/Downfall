using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class RazorSharp : AwakenedCardModel
{
    public RazorSharp() : base(0, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(typeof(PlumeJab));
        WithPower<RazorSharpPower>(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<PlumeJab>(Owner, PileType.Draw, 2);
        await MyCommonActions.ApplySelf<RazorSharpPower>(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<RazorSharpPower>().UpgradeValueBy(1);
    }
}