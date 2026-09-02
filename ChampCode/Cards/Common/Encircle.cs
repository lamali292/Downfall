using BaseLib.Extensions;
using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Encircle : ChampCardModel
{
    public Encircle() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(6, 3);
        WithPower<EnergyNextTurnPower>(1, false);
        WithEnergy(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
    }
}