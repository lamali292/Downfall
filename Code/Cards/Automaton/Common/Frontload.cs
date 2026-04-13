using BaseLib.Utils;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload()
    : AutomatonCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self), ICompilable, IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

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