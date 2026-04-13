using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Torchbearer : CollectorCardModel
{
    public Torchbearer() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.Summon<Osty>(ctx, Owner, 20, this);
    }
    
    public override bool ShouldAllowHitting(Creature creature)
    {
        if (creature.Monster is Osty)
            return creature.IsAlive;
        return base.ShouldAllowHitting(creature);
    }

    protected override void OnUpgrade()
    {
        
    }
}