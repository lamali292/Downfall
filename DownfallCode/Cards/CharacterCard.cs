using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Random;

namespace Downfall.DownfallCode.Cards;

[Pool(typeof(TokenCardPool))]
#pragma warning disable
public class CharacterCard() : ConstructedCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
#pragma warning restore
{
    internal CharacterModel? CharacterModel;
    public CardModel? RandomCommonCard;
    public CardModel? RandomRareCard;
    public CardModel? RandomUncommonCard;

    protected override bool IsPlayable => false;

    public static CharacterCard Create(CharacterModel characterModel)
    {
        var a = ModelDb.Card<CharacterCard>().ToMutable();
        if (a is not CharacterCard characterCard) throw new Exception("CharacterCard model is not a CharacterCard");
        characterCard.CharacterModel = characterModel;
        characterCard._pool = characterModel.CardPool;
        characterCard.RandomCommonCard = characterModel.CardPool.AllCards.Where(e => e.Rarity == CardRarity.Common)
            .TakeRandom(1, Rng.Chaotic).FirstOrDefault();
        characterCard.RandomUncommonCard = characterModel.CardPool.AllCards.Where(e => e.Rarity == CardRarity.Uncommon)
            .TakeRandom(1, Rng.Chaotic).FirstOrDefault();
        characterCard.RandomRareCard = characterModel.CardPool.AllCards.Where(e => e.Rarity == CardRarity.Rare)
            .TakeRandom(1, Rng.Chaotic).FirstOrDefault();
        NCard.FindOnTable(characterCard)?.Reload();
        return characterCard;
    }

    private static decimal Calc(CardModel arg1, Creature? arg2)
    {
        throw new NotImplementedException();
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Title), MethodType.Getter)]
public static class CardModelTitlePatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref string __result)
    {
        if (__instance is CharacterCard { CharacterModel: not null } characterCard)
            __result = new LocString("characters", characterCard.CharacterModel.CharacterSelectTitle)
                .GetFormattedText();
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Description), MethodType.Getter)]
public static class CardModelDescriptionPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is CharacterCard { CharacterModel: not null } characterCard)
            __result = new LocString("characters", characterCard.CharacterModel.CharacterSelectDesc);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.Create))]
public static class NCardCreatePatch
{
    private static bool Prefix(CardModel card, ModelVisibility visibility, ref NCard? __result)
    {
        if (card is not CharacterCard) return true;
        var scene = ResourceLoader.Load<PackedScene>(NCard._scenePath);
        var ncard = scene.Instantiate<NCard>();
        ncard.Model = card;
        ncard.Visibility = visibility;
        __result = ncard;
        return false;
    }
}

[HarmonyPatch(typeof(NodePool), nameof(NodePool.Free), typeof(IPoolable))]
public static class NodePoolFreePatch
{
    private static bool Prefix(IPoolable poolable)
    {
        if (poolable is not NCard { Model: CharacterCard } ncard) return true;
        ncard.QueueFree();
        return false;
    }
}

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPortraitPatch
{
    private static void Postfix(NCard __instance)
    {
        var portrait = __instance.GetNode<Control>("%Portrait");
        if (portrait == null) return;

        foreach (var child in portrait.GetChildren().Where(c => c.Name.ToString().StartsWith("_composite_")))
            child.QueueFree();

        if (__instance.Model is not CharacterCard fc) return;


        List<CardModel?> cards = [fc.RandomCommonCard, fc.RandomUncommonCard, fc.RandomRareCard];
        var textures = cards.Select(c => c?.Portrait).Where(t => t != null).Cast<Texture2D>().ToList();
        if (textures.Count == 0) return;

        for (var i = 0; i < textures.Count; i++)
        {
            var src = textures[i];
            var w = src.GetWidth();
            var h = src.GetHeight();
            var sliceW = w / textures.Count;

            var atlas = new AtlasTexture
            {
                Atlas = src,
                Region = new Rect2(i * sliceW, 0, sliceW, h)
            };

            portrait.AddChild(new TextureRect
            {
                Name = $"_composite_{i}",
                Texture = atlas,
                AnchorLeft = (float)i / textures.Count,
                AnchorRight = (float)(i + 1) / textures.Count,
                AnchorTop = 0,
                AnchorBottom = 1,
                OffsetLeft = 0, OffsetRight = 0, OffsetTop = 0, OffsetBottom = 0,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.Scale,
                MouseFilter = Control.MouseFilterEnum.Ignore
            });
        }
    }
}