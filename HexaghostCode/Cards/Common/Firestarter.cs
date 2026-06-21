using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Firestarter : HexaghostCardModel
{
    public Firestarter() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(5, 2);
        WithPower<SoulBurnPower>(5, 2);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }
}