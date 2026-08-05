using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class Congregation : AwakenedCardModel
{
    public Congregation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithEnergy(3, 1);
        WithKeyword(CardKeyword.Exhaust);
        this.WithTip<Void>();
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var players = CombatState!.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true });
        foreach (var creature in players)
        {
            var player = creature.Player;
            if (player == null) continue;
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
            var card = CombatState.CreateCard<Void>(player);
            var combat = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Owner);
            if (LocalContext.IsMe(creature))
                CardCmd.PreviewCardPileAdd(combat);
            
        }
     
    }
}