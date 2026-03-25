using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Basic;

[Pool(typeof(AutomatonCardPool))]
public sealed class Goto()
    : AutomatonCardModel(1, CardType.Skill, CardRarity.Basic, TargetType.Self), ICompilable, IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new CardsVar("Compile", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile)
    ];


    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard function, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        if (!forGameplay) return;
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature,
            DynamicVars["Compile"].BaseValue,
            Owner.Creature,
            this);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, cardPlay.Card.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars["Compile"].UpgradeValueBy(1m);
    }
}