using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class SigilOfVictory : ChampCardModel
{
    public SigilOfVictory() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithRepeat(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = ChampModel.GetStanceModel(Owner);
        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
            await a.SkillBonus(ctx);
    }
}