using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class RazorSharp : AwakenedCardModel
{
    public RazorSharp() : base(0, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithTip(typeof(PlumeJab));
        WithPower<RazorSharpPower>(1, 1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<PlumeJab>(Owner, PileType.Draw, 2);
        await CommonActions.ApplySelf<RazorSharpPower>(ctx, this);
    }
}