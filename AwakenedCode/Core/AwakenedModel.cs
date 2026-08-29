using Awakened.AwakenedCode.Displays;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Core;

public class AwakenedModel() : CustomSingletonModel(HookType.Combat)
{
    private static readonly PlayerField<int> AwakenMeter = new(() => 0);
    private static readonly PlayerField<bool> AwakenDispatched = new(() => false);
    private static readonly PlayerField<bool> InitializedSpellbooks = new(() => false);

    public override Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players) AwakenedCmd.RefreshSpellbook(player);
        return Task.CompletedTask;
    }


    public static bool IsAwakened(Player? player)
    {
        return player != null && AwakenMeter.Get(player) >= 7;
    }

    public static bool MarkAwakened(Player player)
    {
        var dispatched = AwakenDispatched.Get(player);
        if (dispatched) return false;
        AwakenDispatched.Set(player, true);
        AwakenedDisplay.RefreshAwakenMeter(player, 7);
        return true;
    }


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner.Character is not Awakened) return;
        if (IsAwakened(owner)) return;
        if (cardPlay.Card.Type != CardType.Power) return;
        var meter = AwakenMeter.Get(owner);
        meter++;
        AwakenedDisplay.RefreshAwakenMeter(cardPlay.Card.Owner, meter);
        AwakenMeter.Set(owner, meter);
        if (IsAwakened(owner))
            await AwakenedCmd.Awaken(owner, ctx);
    }


    internal static void SetupAwakenedCombatUi(CombatState state)
    {
        foreach (var player in state.Players)
        {
            if (player.Character is not Awakened) continue;
            //AwakenedDisplay.RefreshSpellDisplays(player);
            AwakenedDisplay.RefreshAwakenMeter(player, 0);
        }
    }
}