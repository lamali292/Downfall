using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class CopyPaste : AutomatonCardModel
{
    public CopyPaste() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip(DownfallTip.Encode);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var sequence = AutomatonCmd.GetSequence(Owner)
            .OfType<AutomatonCardModel>()
            .ToList();

        foreach (var dupe in sequence.Select(card => card.CreateDupe()))
        {
            if (dupe is not AutomatonCardModel model) continue;
            model.SkipEncode = true;
            await CardCmd.AutoPlay(ctx, model, null);
        }
    }
}