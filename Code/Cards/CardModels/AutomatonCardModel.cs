using BaseLib.Extensions;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.Code.Cards.CardModels;

public abstract class AutomatonCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel(cost, type, rarity, targetType)
{
    public sealed override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<Character.Automaton>();

    public bool SkipEncode { get; set; }
    public bool SuppressCompileError { get; set; }


    public virtual void ApplyToFunctionPreview(FunctionCard card)
    {
    }

    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayEffect(ctx, cardPlay);
        if (this is IEncodable encodable)
        {
            await encodable.PlayEncodableEffect(ctx, cardPlay, EncodeContext.Direct);
            if (!SkipEncode && encodable.AutoEncode) await encodable.Encode(ctx, cardPlay);
        }
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        if (this is IEncodable encodable)
        {
            var encode = encodable.EncodeLocString;
            if (encode != null)
                description.Add("encode", encode);
        }

        if (this is ICompilable compilable)
        {
            var compile = compilable.CompileLocString;
            if (compile != null)
                description.Add("compile", compile);
        }

        if (this is not ICompilableError compilableError) return;
        var compileError = compilableError.CompileErrorLocString;
        if (compileError != null)
            description.Add("error", compileError);
    }
}