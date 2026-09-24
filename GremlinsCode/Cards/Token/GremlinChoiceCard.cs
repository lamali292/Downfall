using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Gremlins.GremlinsCode.Cards.Token;

/// <summary>
///     Pure UI card used to represent a bench Gremlin in the swap selection grid (<see cref="GremlinsCmd" />). Never
///     played or added to a pile; a fresh mutable instance is created per bench Gremlin purely so
///     <see cref="MegaCrit.Sts2.Core.Commands.CardSelectCmd.FromSimpleGrid" /> has something to show and select.
/// </summary>
[Pool(typeof(TokenCardPool))]
public class GremlinChoiceCard : GremlinsCardModel
{
    internal Creature? Gremlin;
    private string? _portraitPath;

    public GremlinChoiceCard() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self,
        showInCardLibrary: false)
    {
    }

    public override string Title => Gremlin?.Monster?.Title.GetFormattedText() ?? "???";
    public override string CustomPortraitPath => _portraitPath ?? base.CustomPortraitPath;
    protected override bool IsPlayable => false;

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public static GremlinChoiceCard Create(Creature gremlin, Player owner)
    {
        if (ModelDb.Card<GremlinChoiceCard>().ToMutable() is not GremlinChoiceCard card)
            throw new Exception("GremlinChoiceCard model is not a GremlinChoiceCard");
        card.Owner = owner;
        card.Gremlin = gremlin;
        card._portraitPath = gremlin.Monster switch
        {
            MadGremlin => "res://Gremlins/images/atlases/card_atlas.sprites/mad_gremlin.tres",
            ShieldGremlin => "res://Gremlins/images/atlases/card_atlas.sprites/shield_gremlin.tres",
            FatGremlin => "res://Gremlins/images/atlases/card_atlas.sprites/fat_gremlin.tres",
            SneakGremlin => "res://Gremlins/images/atlases/card_atlas.sprites/sneaky_gremlin.tres",
            WizardGremlin => "res://Gremlins/images/atlases/card_atlas.sprites/gremlin_wizard.tres",
            _ => null
        };
        return card;
    }
}
