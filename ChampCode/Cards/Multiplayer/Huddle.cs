using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Multiplayer;

[Pool(typeof(ChampCardPool))]
public class Huddle : ChampCardModel
{
    public Huddle() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithPower<VigorPower>(6, 2);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var creature in Owner.GetAllPlayers())
            await CommonActions.Apply<VigorPower>(ctx, creature.Creature, this);
    }
}