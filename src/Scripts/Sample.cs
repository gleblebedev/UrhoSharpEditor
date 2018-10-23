using System.Diagnostics;
using Urho;
using Urho.Resources;

public class Sample : Application
{
    static Sample()
    {
        UnhandledException += Application_UnhandledException1;
    }

    public Sample(ApplicationOptions options) : base(options)
    {
    }

    protected float TouchSensitivity { get; set; } = 2;

    protected MonoDebugHud MonoDebugHud { get; set; }
    protected float Yaw { get; set; }
    protected float Pitch { get; set; }
    protected bool TouchEnabled { get; set; }

    protected Node CameraNode { get; set; }

    protected Scene Scene { get; set; }

    /// <summary>
    ///     Set custom Joystick layout for mobile platforms
    /// </summary>
    protected virtual string JoystickLayoutPatch => "";
    //"<patch>" +
    //"    <remove sel=\"/element/element[./attribute[@name='Name' and @value='Button0']]/attribute[@name='Is Visible']\" />" +
    //"    <replace sel=\"/element/element[./attribute[@name='Name' and @value='Button0']]/element[./attribute[@name='Name' and @value='Label']]/attribute[@name='Text']/@value\">1st/3rd</replace>" +
    //"    <add sel=\"/element/element[./attribute[@name='Name' and @value='Button0']]\">" +
    //"        <element type=\"Text\">" +
    //"            <attribute name=\"Name\" value=\"KeyBinding\" />" +
    //"            <attribute name=\"Text\" value=\"F\" />" +
    //"        </element>" +
    //"    </add>" +
    //"    <remove sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]/attribute[@name='Is Visible']\" />" +
    //"    <replace sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]/element[./attribute[@name='Name' and @value='Label']]/attribute[@name='Text']/@value\">Jump</replace>" +
    //"    <add sel=\"/element/element[./attribute[@name='Name' and @value='Button1']]\">" +
    //"        <element type=\"Text\">" +
    //"            <attribute name=\"Name\" value=\"KeyBinding\" />" +
    //"            <attribute name=\"Text\" value=\"SPACE\" />" +
    //"        </element>" +
    //"    </add>" +
    //"</patch>";

    private static void Application_UnhandledException1(object sender, UnhandledExceptionEventArgs e)
    {
        if (Debugger.IsAttached && !e.Exception.Message.Contains("BlueHighway.ttf"))
            Debugger.Break();
        e.Handled = true;
    }


    protected override void Start()
    {
        base.Start();

        if (Platform == Platforms.Android ||
            Platform == Platforms.iOS ||
            Options.TouchEmulation)
            InitTouchInput();
        Input.Enabled = true;

        MonoDebugHud = new MonoDebugHud(this);
        MonoDebugHud.Show();

        CreateScene();
        SetupViewport();
    }

    private void SetupViewport()
    {
        var renderer = Renderer;
        renderer.SetViewport(0, new Viewport(Context, Scene, CameraNode.GetComponent<Camera>(), null));
    }

    private void CreateScene()
    {
        var cache = ResourceCache;
        Scene = new Scene();
        Scene.LoadXml(FileSystem.ProgramDir + "Data/Scenes/Scene.xml");
        CameraNode = Scene.GetChild("MainCamera", true);
    }

    private void InitTouchInput()
    {
        TouchEnabled = true;
        var layout = ResourceCache.GetXmlFile("UI/ScreenJoystick.xml");
        if (!string.IsNullOrEmpty(JoystickLayoutPatch))
        {
            var patchXmlFile = new XmlFile();
            patchXmlFile.FromString(JoystickLayoutPatch);
            layout.Patch(patchXmlFile);
        }
        var screenJoystickIndex = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));
        Input.SetScreenJoystickVisible(screenJoystickIndex, true);
    }

    protected override void OnUpdate(float timeStep)
    {
        SimpleMoveCamera3D(timeStep);
        MoveCameraByTouches(timeStep);
        base.OnUpdate(timeStep);
    }

    /// <summary>
    ///     Move camera for 3D samples
    /// </summary>
    protected void SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f)
    {
        if (CameraNode == null)
            return;

        const float mouseSensitivity = .1f;

        if (UI.FocusElement != null)
            return;

        var mouseMove = Input.MouseMove;
        Yaw += mouseSensitivity * mouseMove.X;
        Pitch += mouseSensitivity * mouseMove.Y;
        Pitch = MathHelper.Clamp(Pitch, -90, 90);

        CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);

        if (Input.GetKeyDown(Key.W)) CameraNode.Translate(Vector3.UnitZ * moveSpeed * timeStep);
        if (Input.GetKeyDown(Key.S)) CameraNode.Translate(-Vector3.UnitZ * moveSpeed * timeStep);
        if (Input.GetKeyDown(Key.A)) CameraNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
        if (Input.GetKeyDown(Key.D)) CameraNode.Translate(Vector3.UnitX * moveSpeed * timeStep);
    }

    protected void MoveCameraByTouches(float timeStep)
    {
        if (!TouchEnabled || CameraNode == null)
            return;

        var input = Input;
        for (uint i = 0, num = input.NumTouches; i < num; ++i)
        {
            var state = input.GetTouch(i);
            if (state.TouchedElement != null)
                continue;

            if (state.Delta.X != 0 || state.Delta.Y != 0)
            {
                var camera = CameraNode.GetComponent<Camera>();
                if (camera == null)
                    return;

                var graphics = Graphics;
                Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
                Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
                CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
            }
            else
            {
                var cursor = UI.Cursor;
                if (cursor != null && cursor.Visible)
                    cursor.Position = state.Position;
            }
        }
    }

    protected override void Stop()
    {
        base.Stop();
    }
}