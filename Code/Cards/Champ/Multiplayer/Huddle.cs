using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class Huddle : ChampCardModel
{
    public Huddle() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<VigorPower>(6, 2);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var creature in CombatState.GetTeammatesOf(Owner.Creature)
                     .Where(c => c is { IsAlive: true, IsPlayer: true }))
            await CommonActions.Apply<VigorPower>(ctx,creature, this);
    }
}