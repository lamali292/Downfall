using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class CultistStrike : AutomatonCardModel
{
    public CultistStrike() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithDamage(CurrentDamage);
        WithVar("Increase", 1, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public int CurrentDamage
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            DynamicVars.Damage.BaseValue = field;
        }
    } = 6;

    [SavedProperty]
    // ReSharper disable once MemberCanBePrivate.Global
    public int IncreasedDamage
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
        var intValue = DynamicVars["Increase"].IntValue;
        BuffFromPlay(intValue);
        if (DeckVersion is not CultistStrike deckVersion) return;
        deckVersion.BuffFromPlay(intValue);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromPlay(int extraDamage)
    {
        IncreasedDamage += extraDamage;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = 6 + IncreasedDamage;
    }
}