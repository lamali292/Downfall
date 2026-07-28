using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class FeatherDance : AwakenedCardModel
{
    public FeatherDance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithCards(2, 1);
        this.WithTip<PlumeJab>();
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
            var cards = new List<CardModel>();
            for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
            {
                cards.Add(CombatState.CreateCard<PlumeJab>(player));
            }
            var combat = await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
            if (LocalContext.IsMe(creature))
                CardCmd.PreviewCardPileAdd(combat);
            
        }
     
    }
}