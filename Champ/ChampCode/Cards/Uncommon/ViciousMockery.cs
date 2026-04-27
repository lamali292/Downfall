using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class ViciousMockery : ChampCardModel
{
    public ViciousMockery() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<VigorPower>(5, 1);
        WithPower<WeakPower>(1, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
        await Owner.ChampStance().SkillBonus(ctx);
    }
}