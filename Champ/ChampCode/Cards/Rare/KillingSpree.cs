using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Rare;

[Pool(typeof(ChampCardPool))]
public class KillingSpree : ChampCardModel
{
    public KillingSpree() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<KillingSpreePower>(1);
        WithVar("Skill", 3, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<KillingSpreePower>(ctx, this);
        for (var i = 0; i < DynamicVars["Skill"].IntValue; i++) await Owner.ChampStance().SkillBonus(ctx);
    }
}