using System.Reflection;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
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
        var selectorScope = CardSelectCmd.UseSelector(new TestCardSelector());

        try
        {
            // Find all methods with [CardTest] in the current assembly
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
                var (combat, player) = await NewCombatAsync(seed, attr.CharacterType, attr.EncounterType);
                var context = new TestContext(combat, player);
                
                try
                {
                    AutoSlayLog.Info($"Running: {testName}");
                    
                    // Create an instance of the class containing the test method
                    var testInstance = Activator.CreateInstance(method.DeclaringType!);
                    
                    // Invoke the async test method
                    var task = (Task)method.Invoke(testInstance, new object[] { context });
                    await task;
                }
                catch (Exception ex)
                {
                    // Unpack TargetInvocationException from Reflection
                    var actualEx = ex.InnerException ?? ex;
                    _failures.Add((testName, actualEx));
                    AutoSlayLog.Error($"[FAILED] {testName}: {actualEx.Message}");
                }
                finally
                {
                    EndCombat();
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


    private void Report()
    {
        if (_failures.Count == 0)
        {
            AutoSlayLog.Action($"[TestRunner]: All tests passed!");
            return;
        }

        AutoSlayLog.Warn($"[TestRunner]: {_failures.Count} test(s) failed:");
    }


    
       private async Task<(CombatState combat, Player player)> NewCombatAsync(string seed, 
           Type? characterType = null, 
           Type? encounterType = null)
    {
        if (CombatManager.Instance.DebugOnlyGetState() != null)
            CombatManager.Instance.Reset(true);
        
        RunManager.Instance.State = null;
        characterType ??= typeof(Ironclad);
        

        var characterModel = (CharacterModel)ModelDb.Get(characterType);
        var playerObj = Player.CreateForNewRun(characterModel, UnlockState.all, 1UL);
        
        _run = RunState.CreateForTest(
            players: [playerObj],
            seed: seed);
        var run = _run;
      
        RunManager.Instance.SetUpTest(_run, new NetSingleplayerGameService(), shouldSave: false);
        LocalContext.NetId = RunManager.Instance.NetService.NetId;var player = run.Players[0];
        
        var encounter = encounterType != null 
            ? ((EncounterModel)ModelDb.Get(encounterType)).ToMutable()
            : ModelDb.AllEncounters.First().ToMutable();
        encounter.DebugRandomizeRng();

        var combat = new CombatState(encounter, run,
            run.Modifiers, run.BadgeModels, run.MultiplayerScalingModel);
        combat.AddPlayer(player);

        if (!encounter.HaveMonstersBeenGenerated)
            encounter.GenerateMonstersWithSlots(run);
        foreach (var (monster, slot) in encounter.MonstersWithSlots)
        {
            monster.AssertMutable();
            var enemy = combat.CreateCreature(monster, CombatSide.Enemy, slot);
            combat.AddCreature(enemy);
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
    }
}