using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.addons.mega_text;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NTopBarCollectorButton : NTopBarButton
{
    private static NTopBarCollectorButton? _instance;
    public static Vector2 ButtonPosition => _instance?.GlobalPosition ?? Vector2.Zero;
    public static Vector2 ButtonSize => _instance?.Size ?? Vector2.Zero;

    private Player? _player;
    private MegaLabel? _countLabel;
    private float _count;
    private Tween? _bumpTween;

    public static void RefreshButton() => _instance?.RefreshCount();

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>("Control/Icon");
        _hsv = (ShaderMaterial)_icon.Material;
        _countLabel = GetNode<MegaLabel>("DeckCardCount");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }

    public void Initialize(Player player)
    {
        _instance = this;
        _player = player;
        RefreshCount();
    }

    
    private float _elapsedTime;
    private float _rockBaseRotation;
    private const float RockSpeed = 4f;
    private const float RockDist = 0.12f;

    public override void _Process(double delta)
    {
        if (!IsScreenOpen) return;
        _elapsedTime += (float)delta * RockSpeed;
        _icon.Rotation = _rockBaseRotation + RockDist * Mathf.Sin(_elapsedTime);
        _rockBaseRotation = (float)Mathf.Lerp(_rockBaseRotation, 0.0, delta);
    }
    
    private void RefreshCount()
    {
        if (_countLabel == null || _player == null) return;
        var count = CollectiblesModel.GetCollectibles(_player).Count;
        if (count > _count)
        {
            _bumpTween?.Kill();
            _bumpTween = CreateTween();
            _bumpTween.TweenProperty(_countLabel, "scale", Vector2.One, 0.5f)
                .From(Vector2.One * 1.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Expo);
            _countLabel.PivotOffset = _countLabel.Size * 0.5f;
            _count = count;
        }
        _countLabel.SetTextAutoSize(count.ToString());
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        if (_player == null || NCapstoneContainer.Instance == null) return;
        if (IsOpen())
            NCapstoneContainer.Instance.Close();
        else
            NCollectiblesViewScreen.ShowScreen(_player);
        UpdateScreenOpen();
    }

    protected override bool IsOpen()
        => NCapstoneContainer.Instance?.CurrentCapstoneScreen is NCollectiblesViewScreen;
}

[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
class PatchTopBarInitialize
{
    [HarmonyPostfix]
    static void AddCollectiblesButton(NTopBar __instance, IRunState runState)
    {
        var localPlayer = LocalContext.GetMe(runState);
        if (localPlayer?.Character is not Character.Collector) return;

        var scene = ResourceLoader.Load<PackedScene>("res://Downfall/scenes/ui/top_bar_collector_button.tscn");
        if (scene == null) return;
        var button = scene.Instantiate<NTopBarCollectorButton>();
        __instance.AddChildSafely(button);
        button.Initialize(localPlayer);

        var essenceScene = ResourceLoader.Load<PackedScene>("res://Downfall/scenes/ui/top_bar_essence.tscn");
        if (essenceScene == null) return;
        var essence = essenceScene.Instantiate<NTopBarEssenceDisplay>();
        __instance.AddChildSafely(essence);
        essence.Initialize(localPlayer); 
        
        Callable.From(() =>
        {
            button.GlobalPosition = __instance.Map.GlobalPosition + new Vector2(-80f, 0);
            essence.GlobalPosition = button.GlobalPosition + new Vector2(-80f, 0);
            
            var map = __instance.GetNodeOrNull<Node>("RightAlignedStuff/Map");
            if (map != null)
            {
                var spacer = new Control();
                spacer.CustomMinimumSize = new Vector2(160, 0);
                spacer.MouseFilter = Control.MouseFilterEnum.Ignore;
                var parent = map.GetParent();
                parent.AddChild(spacer);
                parent.MoveChild(spacer, map.GetIndex());
            }
        }).CallDeferred();
    }
}