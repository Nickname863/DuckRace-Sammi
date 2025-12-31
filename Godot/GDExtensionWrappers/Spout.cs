#pragma warning disable CS0109
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace GDExtension.Wrappers;

public partial class Spout : RefCounted
{

    private new static readonly StringName NativeName = new StringName("Spout");

    [Obsolete("Wrapper types cannot be constructed with constructors (it only instantiate the underlying Spout object), please use the Instantiate() method instead.")]
    protected Spout() { }

    private static CSharpScript _wrapperScriptAsset;

    /// <summary>
    /// Try to cast the script on the supplied <paramref name="godotObject"/> to the <see cref="Spout"/> wrapper type,
    /// if no script has attached to the type, or the script attached to the type does not inherit the <see cref="Spout"/> wrapper type,
    /// a new instance of the <see cref="Spout"/> wrapper script will get attaches to the <paramref name="godotObject"/>.
    /// </summary>
    /// <remarks>The developer should only supply the <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</remarks>
    /// <param name="godotObject">The <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</param>
    /// <returns>The existing or a new instance of the <see cref="Spout"/> wrapper script attached to the supplied <paramref name="godotObject"/>.</returns>
    public new static Spout Bind(GodotObject godotObject)
    {
#if DEBUG
        if (!IsInstanceValid(godotObject))
            throw new InvalidOperationException("The supplied GodotObject instance is not valid.");
#endif
        if (godotObject is Spout wrapperScriptInstance)
            return wrapperScriptInstance;

#if DEBUG
        var expectedType = typeof(Spout);
        var currentObjectClassName = godotObject.GetClass();
        if (!ClassDB.IsParentClass(expectedType.Name, currentObjectClassName))
            throw new InvalidOperationException($"The supplied GodotObject ({currentObjectClassName}) is not the {expectedType.Name} type.");
#endif

        if (_wrapperScriptAsset is null)
        {
            var scriptPathAttribute = typeof(Spout).GetCustomAttributes<ScriptPathAttribute>().FirstOrDefault();
            if (scriptPathAttribute is null) throw new UnreachableException();
            _wrapperScriptAsset = ResourceLoader.Load<CSharpScript>(scriptPathAttribute.Path);
        }

        var instanceId = godotObject.GetInstanceId();
        godotObject.SetScript(_wrapperScriptAsset);
        return (Spout)InstanceFromId(instanceId);
    }

    /// <summary>
    /// Creates an instance of the GDExtension <see cref="Spout"/> type, and attaches a wrapper script instance to it.
    /// </summary>
    /// <returns>The wrapper instance linked to the underlying GDExtension "Spout" type.</returns>
    public new static Spout Instantiate() => Bind(ClassDB.Instantiate(NativeName).As<GodotObject>());

    public enum GlFormat
    {
        Rgba = 6408,
        Bgra = 32993,
        BgraExt = 32993,
    }

    public new static class GDExtensionMethodName
    {
        public new static readonly StringName SetSenderName = "set_sender_name";
        public new static readonly StringName SetSenderFormat = "set_sender_format";
        public new static readonly StringName ReleaseSender = "release_sender";
        public new static readonly StringName SendFbo = "send_fbo";
        public new static readonly StringName SendTexture = "send_texture";
        public new static readonly StringName SendImage = "send_image";
        public new static readonly StringName GetName = "get_name";
        public new static readonly StringName GetWidth = "get_width";
        public new static readonly StringName GetHeight = "get_height";
        public new static readonly StringName GetFps = "get_fps";
        public new static readonly StringName GetFrame = "get_frame";
        public new static readonly StringName GetHandle = "get_handle";
        public new static readonly StringName GetCpu = "get_cpu";
        public new static readonly StringName GetGldx = "get_gldx";
        public new static readonly StringName SetReceiverName = "set_receiver_name";
        public new static readonly StringName ReleaseReceiver = "release_receiver";
        public new static readonly StringName ReceiveTexture = "receive_texture";
        public new static readonly StringName ReceiveImage = "receive_image";
        public new static readonly StringName IsUpdated = "is_updated";
        public new static readonly StringName IsSenderConnected = "is_sender_connected";
        public new static readonly StringName IsFrameNew = "is_frame_new";
        public new static readonly StringName GetSenderName = "get_sender_name";
        public new static readonly StringName GetSenderWidth = "get_sender_width";
        public new static readonly StringName GetSenderHeight = "get_sender_height";
        public new static readonly StringName GetSenderFormat = "get_sender_format";
        public new static readonly StringName GetSenderFps = "get_sender_fps";
        public new static readonly StringName GetSenderFrame = "get_sender_frame";
        public new static readonly StringName GetSenderHandle = "get_sender_handle";
        public new static readonly StringName GetSenderCpu = "get_sender_cpu";
        public new static readonly StringName GetSenderGldx = "get_sender_gldx";
        public new static readonly StringName SelectSender = "select_sender";
        public new static readonly StringName GetSenderCount = "get_sender_count";
        public new static readonly StringName GetSender = "get_sender";
    }

    public new void SetSenderName(string senderName) => 
        Call(GDExtensionMethodName.SetSenderName, [senderName]);

    public new void SetSenderFormat() => 
        Call(GDExtensionMethodName.SetSenderFormat, []);

    public new void ReleaseSender() => 
        Call(GDExtensionMethodName.ReleaseSender, []);

    public new bool SendFbo(long fboId, long width, long height, bool invert = true) => 
        Call(GDExtensionMethodName.SendFbo, [fboId, width, height, invert]).As<bool>();

    public new bool SendTexture(long textureId, long textureTarget, long width, long height, bool invert = true, long hostFbo = 0) => 
        Call(GDExtensionMethodName.SendTexture, [textureId, textureTarget, width, height, invert, hostFbo]).As<bool>();

    public new bool SendImage(Image image, long width, long height, Spout.GlFormat glFormat = Spout.GlFormat.Rgba, bool invert = true) => 
        Call(GDExtensionMethodName.SendImage, [image, width, height, Variant.From(glFormat), invert]).As<bool>();

    public new string GetName() => 
        Call(GDExtensionMethodName.GetName, []).As<string>();

    public new long GetWidth() => 
        Call(GDExtensionMethodName.GetWidth, []).As<long>();

    public new long GetHeight() => 
        Call(GDExtensionMethodName.GetHeight, []).As<long>();

    public new double GetFps() => 
        Call(GDExtensionMethodName.GetFps, []).As<double>();

    public new long GetFrame() => 
        Call(GDExtensionMethodName.GetFrame, []).As<long>();

    public new void GetHandle() => 
        Call(GDExtensionMethodName.GetHandle, []);

    public new bool GetCpu() => 
        Call(GDExtensionMethodName.GetCpu, []).As<bool>();

    public new bool GetGldx() => 
        Call(GDExtensionMethodName.GetGldx, []).As<bool>();

    public new void SetReceiverName(string senderName) => 
        Call(GDExtensionMethodName.SetReceiverName, [senderName]);

    public new void ReleaseReceiver() => 
        Call(GDExtensionMethodName.ReleaseReceiver, []);

    public new bool ReceiveTexture(long textureId = 0, long textureTarget = 0, bool invert = false, long hostFbo = 0) => 
        Call(GDExtensionMethodName.ReceiveTexture, [textureId, textureTarget, invert, hostFbo]).As<bool>();

    public new bool ReceiveImage(Image image, Spout.GlFormat glFormat, bool invert = false, long hostFbo = 0) => 
        Call(GDExtensionMethodName.ReceiveImage, [image, Variant.From(glFormat), invert, hostFbo]).As<bool>();

    public new bool IsUpdated() => 
        Call(GDExtensionMethodName.IsUpdated, []).As<bool>();

    public new bool IsSenderConnected() => 
        Call(GDExtensionMethodName.IsSenderConnected, []).As<bool>();

    public new bool IsFrameNew() => 
        Call(GDExtensionMethodName.IsFrameNew, []).As<bool>();

    public new string GetSenderName() => 
        Call(GDExtensionMethodName.GetSenderName, []).As<string>();

    public new long GetSenderWidth() => 
        Call(GDExtensionMethodName.GetSenderWidth, []).As<long>();

    public new long GetSenderHeight() => 
        Call(GDExtensionMethodName.GetSenderHeight, []).As<long>();

    public new void GetSenderFormat() => 
        Call(GDExtensionMethodName.GetSenderFormat, []);

    public new double GetSenderFps() => 
        Call(GDExtensionMethodName.GetSenderFps, []).As<double>();

    public new long GetSenderFrame() => 
        Call(GDExtensionMethodName.GetSenderFrame, []).As<long>();

    public new void GetSenderHandle() => 
        Call(GDExtensionMethodName.GetSenderHandle, []);

    public new bool GetSenderCpu() => 
        Call(GDExtensionMethodName.GetSenderCpu, []).As<bool>();

    public new bool GetSenderGldx() => 
        Call(GDExtensionMethodName.GetSenderGldx, []).As<bool>();

    public new void SelectSender() => 
        Call(GDExtensionMethodName.SelectSender, []);

    public new long GetSenderCount() => 
        Call(GDExtensionMethodName.GetSenderCount, []).As<long>();

    public new string GetSender(long index = 0) => 
        Call(GDExtensionMethodName.GetSender, [index]).As<string>();

}
