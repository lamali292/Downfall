using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Champ.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class ShowOff : ChampCardModel
{
    public ShowOff() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithCards(3, 1);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Owner.ChampStance().SkillBonus();
        if (cardPlay.Target?.Player == null) return;
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, cardPlay.Target.Player);
    }
}