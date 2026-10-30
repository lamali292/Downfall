using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
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
    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var creature in Owner.AllTeammates)
            await CommonActions.Apply<VigorPower>(ctx, creature.Creature, this);
    }
}