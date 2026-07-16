using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Cards.Common;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Philosophize : AutomatonCardModel
{
    public Philosophize() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        this.WithPower<PhilosophizePower>(3, 2, false);
        this.WithTip<StrengthPower>();
    }
    
    protected override Artist Artist => Artist.Get<Opal>();

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        CardModel card, bool fromHandDraw)
    {
        if (CombatState == null || card != this) return;

        await PowerCmd.Apply<PhilosophizePower>(choiceContext, Owner.Creature,
            DynamicVars.Power<PhilosophizePower>().BaseValue * await GeneratePlayCount(CombatState, null),
            Owner.Creature, this);
    }
}

public class PhilosophizePower : CustomTemporaryPowerModelWrapper<Philosophize, StrengthPower>;