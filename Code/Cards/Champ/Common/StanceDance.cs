using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Champ.Basic;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class StanceDance : ChampCardModel
{
    public StanceDance() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.SelectStanceToEnter(ctx, Owner);
        var stance = Owner.ChampStance();
        await stance.SkillBonus();
        if (IsUpgraded) await stance.SkillBonus();
    }
}

