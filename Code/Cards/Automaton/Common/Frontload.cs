using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload : AutomatonCardModel, ICompilable, IEncodable
{
    public Frontload() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
        WithTip(CardKeyword.Retain);
    }

    public Task OnCompile(PlayerChoiceContext ctx, FunctionCard function, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        function.AddKeyword(CardKeyword.Retain);
        return Task.CompletedTask;
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }


    public override void ApplyToFunctionPreview(FunctionCard card)
    {
        card.AddKeyword(CardKeyword.Retain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}