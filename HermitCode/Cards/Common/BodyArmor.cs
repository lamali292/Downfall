using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class BodyArmor : HermitCardModel
{
    public BodyArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(5, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1);
        var selected = (await CardSelectCmd.FromHandForDiscard(
            ctx, Owner, prefs, null, this)).FirstOrDefault();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        if (selected == null) return;
        await CardCmd.Discard(ctx, selected);
        if (selected.Type == CardType.Attack) return;
        await CommonActions.CardBlock(this, play);
    }
}