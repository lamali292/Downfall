using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class Flashbang : HexaghostCardModel
{
    public Flashbang() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 1);
        WithPower<TemporaryStrengthDownPower>(2, 1);
        WithPower<WeakPower>(1, 1);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!HexaghostCmd.IsIgnited(Owner)) return;
        await CommonActions.Apply<TemporaryStrengthDownPower>(ctx, this, cardPlay);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}