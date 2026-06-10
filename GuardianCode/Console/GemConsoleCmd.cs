using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
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

    public override string Args => "<gem-id:string> <hand-index:int>";

    public override string Description =>
        "Add a gem to a card in hand by index (0 is leftmost). Example: gem RUBY 0";

    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (!RunManager.Instance.IsInProgress)
            return new CmdResult(false, "A run is currently not in progress!");

        if (issuingPlayer == null)
            return new CmdResult(false, "No player context available!");

        if (args.Length < 2)
            return new CmdResult(false, "Usage: gem <gem-id> <hand-index>");

        var gemName = args[0].ToUpperInvariant();
        var gem = GuardianModelDb.AllGems.FirstOrDefault(g => g.Id.Entry == gemName)?.ToMutable();

        if (gem == null)
            return new CmdResult(false, $"Gem '{gemName}' not found.");

        if (!int.TryParse(args[1], out var handIndex))
            return new CmdResult(false, $"Arg 2 must be hand index (int), got '{args[1]}'.");

        var pile = PileType.Hand.GetPile(issuingPlayer);
        var count = pile.Cards.Count;

        if (handIndex < 0 || handIndex >= count)
            return new CmdResult(false, $"Invalid hand index {handIndex}. Valid range: 0-{count - 1}.");

        var card = pile.Cards[handIndex];

        if (card is not IGemSocketCard guardianCard)
            return new CmdResult(false, $"Card at index {handIndex} is not a Guardian card!");

        if (guardianCard.GemCount >= guardianCard.GemSlots)
            return new CmdResult(false,
                $"Card {card.Id.Entry} already has maximum gems ({guardianCard.GemSlots})!");

        guardianCard.AddGem(gem);
        GuardianMainFile.Logger.Info($"Added gem to card: {card.Title} ({guardianCard.GetHashCode()})");
        GuardianMainFile.Logger.Info($"Gem's card reference: {gem.Card?.Title} ({gem.Card?.GetHashCode()})");
        var a = NCard.FindOnTable(card);
        a?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
        a?.ReloadOverlay();

        return new CmdResult(true, $"Added gem '{gem.Id.Entry}' to '{card.Title}' at index {handIndex}.");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        switch (args.Length)
        {
            // First arg = gem id
            case <= 1:
            {
                var gemNames = GuardianModelDb.AllGems.Select(g => g.Id.Entry).ToList();
                return CompleteArgument(gemNames, Array.Empty<string>(), args.FirstOrDefault() ?? "");
            }
            // Second arg = hand index
            case 2 when RunManager.Instance.IsInProgress && player != null:
            {
                var count = player.GetHand().Count;
                if (count > 0)
                    return CompleteArgument(
                        Enumerable.Range(0, count).Select(i => i.ToString()).ToList(),
                        [args[0]],
                        args[1]);
                break;
            }
        }

        return new CompletionResult
        {
            Type = CompletionType.Argument,
            ArgumentContext = CmdName
        };
    }
}