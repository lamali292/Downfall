using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Uncommon;

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
        await CommonActions.ApplySelf<VigorPower>(this);
        await CommonActions.Apply<WeakPower>(cardPlay.Target, this);
        await Owner.ChampStance().SkillBonus();
    }
}