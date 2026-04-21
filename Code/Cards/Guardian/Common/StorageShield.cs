using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class StorageShield : GuardianCardModel
{
    public StorageShield() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(12, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.DebuffDown(Owner.Creature);
    }
}