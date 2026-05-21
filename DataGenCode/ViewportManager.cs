using Godot;

namespace DataGen.DataGenCode;

public partial class ViewportManager : Node
{
    private const int NumViewportsAvailable = 5;
    private static ViewportManager _inst = new();
    private static readonly Queue<DrawRequest> DrawQueue = [];
    private int _nextViewport;

    private Vp[] _viewports = new Vp[NumViewportsAvailable];

    private bool AllViewportsBusy()
    {
        return _nextViewport >= NumViewportsAvailable;
    }

    public override void _Ready()
    {
        Performance.AddCustomMonitor("ViewportDrawer/num_viewports", Callable.From(() => _nextViewport));
        _inst = this;
        for (var i = 0; i < _viewports.Length; i++)
        {
            Vp viewport = new();
            AddChild(viewport);
            _viewports[i] = viewport;
        }
    }

    public static void AddToTree(SceneTree tree)
    {
        if (_inst.IsInsideTree()) return;
        tree.Root.AddChild(_inst);
    }

    public override void _Process(double delta)
    {
        _nextViewport = -1;
        while (_viewports[++_nextViewport].Doing && _nextViewport + 1 < NumViewportsAvailable) ;
        while (DrawQueue.Count > 0 && TakeRequest()) ;
    }

    private bool TakeRequest()
    {
        if (AllViewportsBusy()) return false;
        while (_viewports[_nextViewport++].Doing)
            if (_nextViewport >= NumViewportsAvailable)
                return false;
        _viewports[_nextViewport - 1].TakeRequest(DrawQueue.Dequeue());
        return true;
    }

    //public static Task<Image> RequestDraw(Vector2 size, Action<VP.Drawer> drawMethod, Action<VP.Drawer> onStart = null) => RequestDraw((Vector2I)size, drawMethod, onStart);
    //public static Task<Image> RequestDraw(Vector2I size, Action<VP.Drawer> drawMethod, Action<VP.Drawer> onStart = null) => RequestDraw(new(size, drawMethod, onStart));
    public static Task<Image> RequestDraw(DrawRequest request)
    {
        DrawQueue.Enqueue(request);
        _inst.TakeRequest();
        return request.Task.Task;
    }

    //public static Task DrawAndSave(string path, Vector2 size, Action<VP.Drawer> drawMethod, Action<VP.Drawer> onStart = null) => DrawAndSave(path, (Vector2I)size, drawMethod, onStart);
    //public static async Task DrawAndSave(string path, Vector2I size, Action<VP.Drawer> drawMethod, Action<VP.Drawer> onStart = null) {
    //    var img = await RequestDraw(size, drawMethod, onStart);
    //    img.SavePng(path);
    //}

    public readonly struct DrawRequest(
        Vector2I dimensions,
        string? path = null,
        Action<Vp.Drawer>? action = null,
        Action<Vp.Drawer>? onStart = null,
        int waitExtraFrames = 0)
    {
        public readonly TaskCompletionSource<Image> Task = new();
        public readonly Vector2I Dimensions = dimensions;
        public readonly string? Path = path;
        public readonly Action<Vp.Drawer>? Action = action;
        public readonly Action<Vp.Drawer>? OnStart = onStart;
        public readonly int WaitExtraFrames = waitExtraFrames;
    }

    public partial class Vp : Window
    {
        private Drawer _drawer = new();
        public bool Doing;

        public override void _Ready()
        {
            base._Ready();
            TransparentBg = true;
            AddChild(_drawer);
            Hide();
            //RenderTargetUpdateMode = UpdateMode.Disabled;
        }

        public async void TakeRequest(DrawRequest request)
        {
            Doing = true;
            Show();
            if (Size != request.Dimensions)
                Size = request.Dimensions;
            //RenderTargetUpdateMode = UpdateMode.Once;
            _drawer.DrawAction = request.Action;
            _drawer.ImageSize = request.Dimensions;
            request.OnStart?.Invoke(_drawer);
            _drawer.QueueRedraw();
            for (var i = 0; i < 1 + request.WaitExtraFrames; i++)
                await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
            var img = GetTexture().GetImage();
            request.Task.TrySetResult(img);
            foreach (var n in _drawer.GetChildren())
            {
                _drawer.RemoveChild(n);
                n.QueueFree();
            }

            Hide();
            Doing = false;
        }

        public partial class Drawer : Node2D
        {
            public Action<Drawer>? DrawAction;
            public Vector2I ImageSize;

            public override void _Draw()
            {
                base._Draw();
                if (DrawAction == null) return;
                DrawSetTransform(Vector2.Zero, 0, Vector2.One);
                DrawAction(this);
            }
        }
    }
}