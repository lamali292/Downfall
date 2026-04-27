using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace Guardian.GuardianCode.Console;

public class GemConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "gem";

    public override string Args => "<hand-index:int> <gem-id:string>";

    public override string Description =>
        "Add a gem to a card in hand by index (0 is leftmost). Example: gem 0 RUBY";

    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (!RunManager.Instance.IsInProgress)
            return new CmdResult(false, "A run is currently not in progress!");

        if (issuingPlayer == null)
            return new CmdResult(false, "No player context available!");

        if (args.Length < 2)
            return new CmdResult(false, "Usage: gem <hand-index> <gem-id>");

        if (!int.TryParse(args[0], out var handIndex))
            return new CmdResult(false, $"Arg 1 must be hand index (int), got '{args[0]}'.");

        var pile = PileType.Hand.GetPile(issuingPlayer);
        var count = pile.Cards.Count;

        if (handIndex < 0 || handIndex >= count)
            return new CmdResult(false, $"Invalid hand index {handIndex}. Valid range: 0-{count - 1}.");

        var card = pile.Cards[handIndex];

        if (card is not GuardianCardModel guardianCard)
            return new CmdResult(false, $"Card at index {handIndex} is not a Guardian card!");

        var gemName = args[1].ToUpperInvariant();
        var gem = GuardianModelDb.AllGems.FirstOrDefault(g => g.Id.Entry == gemName);

        if (gem == null)
            return new CmdResult(false, $"Gem '{gemName}' not found.");

        if (guardianCard.Gems.Count >= guardianCard.GemSlots)
            return new CmdResult(false,
                $"Card {guardianCard.Id.Entry} already has maximum gems ({guardianCard.GemSlots})!");

        guardianCard.AddGem(gem);
        var a = NCard.FindOnTable(card);
        a?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        a?.ReloadOverlay();
        ;
        return new CmdResult(true, $"Added gem '{gem.Id.Entry}' to '{card.Title}' at index {handIndex}.");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length <= 1 && RunManager.Instance.IsInProgress && player != null)
        {
            var count = PileType.Hand.GetPile(player).Cards.Count;
            if (count > 0)
                return CompleteArgument(
                    Enumerable.Range(0, count).Select(i => i.ToString()).ToList(),
                    Array.Empty<string>(),
                    args.FirstOrDefault() ?? "");
        }

        if (args.Length != 2)
            return new CompletionResult
            {
                Type = CompletionType.Argument,
                ArgumentContext = CmdName
            };
        var gemNames = GuardianModelDb.AllGems.Select(g => g.Id.Entry).ToList();

        return CompleteArgument(gemNames, [args[0]], args[1]);
    }
}