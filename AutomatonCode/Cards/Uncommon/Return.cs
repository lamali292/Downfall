using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Return : AutomatonCardModel
{
    public Return() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithEnergy(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var selected =
            (await DownfallCardCmd.SelectFromCards(ctx, Owner.GetDiscard(),
                DownfallCardSelectorPrefs.ToTopSelectionPrompt, this)).FirstOrDefault();

        if (selected != null) await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);

        await CommonActions.ApplySelf<EnergyNextTurnPower>(ctx, this, DynamicVars.Energy.BaseValue);
    }
}