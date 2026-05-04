using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class RopeADope : ChampCardModel
{
    public RopeADope() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithFinisher();
        WithBlock(8, 2);
        WithPower<EnergyNextTurnPower>(1, 1);
        WithEnergy(1, 1);
        WithPower<DrawCardsNextTurnPower>(2);
        WithCards(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}