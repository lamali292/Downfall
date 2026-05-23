using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.DynamicVars;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace Automaton.AutomatonCode.Cards;

public abstract class AutomatonCardModel : DownfallCardModel<Core.Automaton>
{
    public AutomatonCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true
    ) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        if (this is IEncodable)
            WithTip(AutomatonTip.Encode);
    }

    public bool SkipEncode { get; set; }
    public bool SuppressCompileError { get; set; }

    protected virtual async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

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
        if (this is not IEncodable encodable) return;
        var encode = encodable.EncodeLocString;
        if (encode != null)
            description.Add("encode", encode);

    }


    protected void WithStash(int baseValue, int upgradeValue = 0)
    {
        WithVars(new StashVar(baseValue).WithUpgrade(upgradeValue));
    }
}