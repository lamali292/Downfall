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
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Blockchain() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IEncodable,
    ICompilable
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BlurPower>(1),
        new("BlurCompilePower", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        IsUpgraded
            ?
            [
                HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
                HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
                HoverTipFactory.FromPower<BlurPower>()
            ]
            :
            [
                HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
                HoverTipFactory.FromPower<BlurPower>()
            ];


    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        if (IsUpgraded)
            await PowerCmd.Apply<BlurPower>(Owner.Creature, DynamicVars["BlurCompilePower"].BaseValue, Owner.Creature,
                this);
    }


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await PowerCmd.Apply<BlurPower>(Owner.Creature, DynamicVars["BlurPower"].BaseValue, Owner.Creature, this);
    }
}