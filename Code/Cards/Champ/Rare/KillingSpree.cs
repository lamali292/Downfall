using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Rare;

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
        await CommonActions.ApplySelf<KillingSpreePower>(this);
        for (var i = 0; i < DynamicVars["Skill"].IntValue; i++) await Owner.ChampStance().SkillBonus();
    }
}