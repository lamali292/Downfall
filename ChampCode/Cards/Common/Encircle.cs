using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Common;

[Pool(typeof(ChampCardPool))]
public class Encircle : ChampCardModel
{
    public Encircle() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(7, 3);
        this.WithGlory(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var attack = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var a = attack.Results.Count();
        await CommonActions.ApplySelf<GloryPower>(ctx, this, a);
    }
}