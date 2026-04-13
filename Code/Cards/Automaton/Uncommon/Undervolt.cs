using BaseLib.Utils;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public sealed class Undervolt() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromCard<Burn>()
    ];


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var combatState = Owner.Creature.CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        await PowerCmd.Apply<StrengthPower>(combatState.Enemies, -DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
        List<CardModel> burns = [
            combatState.CreateCard<Burn>(Owner),
            combatState.CreateCard<Burn>(Owner)
        ];
        await CardPileCmd.AddGeneratedCardsToCombat(burns, PileType.Hand, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1);
    }
}