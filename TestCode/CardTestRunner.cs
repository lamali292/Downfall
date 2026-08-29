using System.Reflection;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace Downfall.TestCode;

public class CardTestRunner
{
    private readonly List<(string testName, Exception ex)> _failures = [];
    private RunState _run = null!;
    
    public async Task RunAllTestsAsync(string seed, CancellationToken ct)
    {
        var wasTestMode = TestMode.IsOn;
        TestMode.IsOn = true;
        var selectorScope = CardSelectCmd.UseSelector(new FirstCardSelector());

        try
        {
            var testMethods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(CardTestAttribute), false).Length > 0)
                .ToList();

            AutoSlayLog.Action($"[TestRunner] Found {testMethods.Count} test cases.");

            foreach (var method in testMethods)
            {
                ct.ThrowIfCancellationRequested();
                var testName = $"{method.DeclaringType?.Name}.{method.Name}";
                var attr = (CardTestAttribute)method.GetCustomAttributes(typeof(CardTestAttribute), false).First();

                try
                {
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        // Single-combat test: one combat, run the method, tear down.
                        await RunSingleTest(method, testName, seed, attr);
                    }
                    else if (typeof(IEnumerable<CardTestCase>).IsAssignableFrom(method.ReturnType))
                    {
                        // Pool test: a fresh combat per card, the old way.
                        await RunPoolTest(method, testName, seed, attr, ct);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"{testName} must return Task or IEnumerable<CardTestCase>, got {method.ReturnType.Name}.");
                    }
                }
                catch (Exception ex)
                {
                    var actualEx = ex.InnerException ?? ex;
                    _failures.Add((testName, actualEx));
                    AutoSlayLog.Error($"[FAILED] {testName}: {actualEx.Message}");
                }
            }
        }
        finally
        {
            selectorScope.Dispose();
            TestMode.IsOn = wasTestMode;
            Report();
        }
    }

    private async Task RunSingleTest(MethodInfo method, string testName, string seed,
                                     CardTestAttribute attr)
    {
        var (combat, player) = await NewCombatAsync(seed, attr.CharacterType, attr.EncounterType);
        var context = new TestContext(combat, player);
        try
        {
            AutoSlayLog.Info($"Running: {testName}");
            var instance = Activator.CreateInstance(method.DeclaringType!);
            var task = (Task)method.Invoke(instance, [context])!;
            await task;
        }
        finally
        {
            EndCombat();
        }
    }

    private async Task RunPoolTest(MethodInfo method, string testName, string seed,
                                   CardTestAttribute attr, CancellationToken ct)
    {
        AutoSlayLog.Info($"Running: {testName}");
        
        var characterType = attr.CharacterType ?? typeof(Ironclad);
        var characterModel = (CharacterModel)ModelDb.Get(characterType);

        var instance = Activator.CreateInstance(method.DeclaringType!);
        var cases = (IEnumerable<CardTestCase>)method.Invoke(instance, [characterModel])!;

        foreach (var testCase in cases)
        {
            ct.ThrowIfCancellationRequested();
            
            var (combat, player) = await NewCombatAsync(seed, attr.CharacterType, attr.EncounterType);
            var context = new TestContext(combat, player);

            try
            {
                await testCase.Run(context);
            }
            catch (Exception ex)
            {
                var actual = ex.InnerException ?? ex;
                _failures.Add(($"{testName}:{testCase.Name}", actual));
                AutoSlayLog.Error($"[FAILED] {testName}:{testCase.Name}: {actual.Message}");
            }
            finally
            {
                EndCombat();
            }
        }
    }

    private void Report()
    {
        if (_failures.Count == 0)
        {
            AutoSlayLog.Action("[TestRunner]: All tests passed!");
            return;
        }
        AutoSlayLog.Warn($"[TestRunner]: {_failures.Count} test(s) failed:");
        foreach (var (name, ex) in _failures)
            AutoSlayLog.Warn($"  - {name}: {ex.Message}");
    }

    private async Task<(CombatState combat, Player player)> NewCombatAsync(
        string seed, Type? characterType = null, Type? encounterType = null)
    {
        if (CombatManager.Instance.DebugOnlyGetState() != null)
            CombatManager.Instance.Reset(true);

        characterType ??= typeof(Ironclad);

        var characterModel = (CharacterModel)ModelDb.Get(characterType);
        var playerObj = Player.CreateForNewRun(characterModel, UnlockState.all, 1UL);

        _run = RunState.CreateForTest(players: [playerObj], seed: seed);
        var run = _run;

        RunManager.Instance.SetUpTest(_run, new NetSingleplayerGameService(), shouldSave: false);
        LocalContext.NetId = RunManager.Instance.NetService.NetId;
        var player = run.Players[0];

        var encounter = encounterType != null
            ? ((EncounterModel)ModelDb.Get(encounterType)).ToMutable()
            : ModelDb.AllEncounters.First().ToMutable();
        encounter.DebugRandomizeRng();

        var combat = new CombatState(encounter, run, run.Modifiers, run.BadgeModels, run.MultiplayerScalingModel);
        combat.AddPlayer(player);

        if (!encounter.HaveMonstersBeenGenerated)
            encounter.GenerateMonstersWithSlots(run);
        foreach (var (monster, slot) in encounter.MonstersWithSlots)
        {
            monster.AssertMutable();
            combat.AddCreature(combat.CreateCreature(monster, CombatSide.Enemy, slot));
        }
        combat.SortEnemiesBySlotName();

        CombatManager.Instance.SetUpCombat(combat);
        CombatManager.Instance.AfterCombatRoomLoaded();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!CombatManager.Instance.IsInProgress && sw.Elapsed < TimeSpan.FromSeconds(10))
            await Task.Yield();
        if (!CombatManager.Instance.IsInProgress)
            throw new InvalidOperationException("Combat never reached IsInProgress after AfterCombatRoomLoaded.");
        while (player.PlayerCombatState?.Phase != PlayerTurnPhase.Play && sw.Elapsed < TimeSpan.FromSeconds(10))
            await Task.Yield();

        return (combat, player);
    }

    private void EndCombat()
    {
        try { CombatManager.Instance.Reset(true); } catch { /* best effort */ }
        
        try
        {
            RunManager.Instance.State = null;
            LocalContext.NetId = null;
        }
        catch { /* best effort */ }
    }
}


/// <summary>Auto-selects the first eligible card(s) for any prompt. For blind "play every card" runs.</summary>
public class FirstCardSelector : ICardSelector
{
    public Task<IEnumerable<CardModel>> GetSelectedCards(
        IEnumerable<CardModel> options, int minSelect, int maxSelect)
    {
        var list = options.ToList();
        //var count = Math.Min(Math.Max(minSelect, 0), Math.Min(maxSelect, list.Count));
        IEnumerable<CardModel> chosen = list.Take(maxSelect).ToList();
        return Task.FromResult(chosen);
    }

    public CardRewardSelection GetSelectedCardReward(
        IReadOnlyList<CardCreationResult> options,
        IReadOnlyList<CardRewardAlternative> alternatives)
    {
        return new CardRewardSelection { card = options.FirstOrDefault()?.Card };
    }
}