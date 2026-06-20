using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class RazorSharp : AwakenedCardModel
{
    public RazorSharp() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithTip<PlumeJab>();
        this.WithPower<RazorSharpPower>(1, 1, false);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCards<PlumeJab>(Owner, PileType.Draw, 2);
        await CommonActions.ApplySelf<RazorSharpPower>(ctx, this);
    }
}