using System.Data;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Nodes;

[GlobalClass]
public partial class NCustomCardHolder : NCardHolder, IPoolable
{
    private CardModel? _baseCard;
    private float _hoverScale;
    private bool _isPreviewingUpgrade;
    private float _smallScale;
    private CardModel? _upgradedCard;

    public override Vector2 SmallScale => Vector2.One * _smallScale;
    protected override Vector2 HoverScale => Vector2.One * _hoverScale;
    public override CardModel? CardModel => _baseCard;

    private static string ScenePath => "res://Downfall/scenes/screens/custom_card_holder.tscn";

    public void OnInstantiated()
    {
    }

    public void OnReturnedFromPool()
    {
        if (!IsNodeReady()) return;
        Position = Vector2.Zero;
        Rotation = 0.0f;
        Scale = Vector2.One;
        Modulate = Colors.White;
        Visible = true;
        SetClickable(true);
        Hitbox.MouseDefaultCursorShape = CursorShape.Arrow;
        _isPreviewingUpgrade = false;
    }

    public void OnFreedToPool()
    {
    }

    public static void InitPool()
    {
        NodePool.Init<NCustomCardHolder>(ScenePath, 30);
    }

    public static NCustomCardHolder? Create(NCard cardNode, float customSmallScale = 1.0f,
        float customHoverScale = 1.0f)
    {
        if (TestMode.IsOn) return null;
        var holder = NodePool.Get<NCustomCardHolder>();
        holder._smallScale = customSmallScale;
        holder._hoverScale = customHoverScale;
        holder.SetCard(cardNode);
        holder.UpdateCardModel();
        holder.UpdateName();
        holder.Scale = holder.SmallScale;
        return holder;
    }

    public override void _Ready()
    {
        var previewingUpgrade = _isPreviewingUpgrade;
        _isPreviewingUpgrade = false;
        SetIsPreviewingUpgrade(previewingUpgrade);
        ConnectSignals();
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        MoveToFront();
    }

    private void UpdateName()
    {
        Name = (StringName)$"CustomCardHolder-{CardNode?.Model?.Id}";
    }

    private void UpdateCardModel()
    {
        var model = CardNode?.Model;
        _baseCard = model;
        if (model is not { IsUpgradable: true }) return;
        _upgradedCard = (CardModel)model.MutableClone();
        _upgradedCard.UpgradeInternal();
        if (!IsNodeReady()) return;
        var previewingUpgrade = _isPreviewingUpgrade;
        _isPreviewingUpgrade = false;
        SetIsPreviewingUpgrade(previewingUpgrade);
    }

    private void SetIsPreviewingUpgrade(bool showUpgradePreview)
    {
        if (!Visible || _baseCard == null || CardNode == null) return;
        if (!_baseCard.IsUpgradable & showUpgradePreview)
            throw new InvalidExpressionException($"{_baseCard.Id} is not upgradable.");
        if (_isPreviewingUpgrade == showUpgradePreview) return;
        if (showUpgradePreview && _upgradedCard != null)
        {
            CardNode.Model = _upgradedCard;
            CardNode.ShowUpgradePreview();
        }
        else
        {
            CardNode.Model = _baseCard;
            CardNode.UpdateVisuals(CardNode.DisplayingPile, CardPreviewMode.Normal);
        }

        _isPreviewingUpgrade = showUpgradePreview;
    }
}