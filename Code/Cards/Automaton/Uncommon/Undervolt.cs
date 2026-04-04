using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Undervolt : AutomatonCardModel
{
    public Undervolt() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Burn));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        await PowerCmd.Apply<StrengthPower>(combatState.Enemies, -DynamicVars.Power<StrengthPower>().BaseValue,
            Owner.Creature, this);
        List<CardModel> burns =
        [
            combatState.CreateCard<Burn>(Owner),
            combatState.CreateCard<Burn>(Owner)
        ];
        await CardPileCmd.AddGeneratedCardsToCombat(burns, PileType.Hand, true);
    }
}