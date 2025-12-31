#pragma warning disable CS0109
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace GDExtension.Wrappers;

public partial class SpoutTexture : ImageTexture
{

    private new static readonly StringName NativeName = new StringName("SpoutTexture");

    [Obsolete("Wrapper types cannot be constructed with constructors (it only instantiate the underlying SpoutTexture object), please use the Instantiate() method instead.")]
    protected SpoutTexture() { }

    private static CSharpScript _wrapperScriptAsset;

    /// <summary>
    /// Try to cast the script on the supplied <paramref name="godotObject"/> to the <see cref="SpoutTexture"/> wrapper type,
    /// if no script has attached to the type, or the script attached to the type does not inherit the <see cref="SpoutTexture"/> wrapper type,
    /// a new instance of the <see cref="SpoutTexture"/> wrapper script will get attaches to the <paramref name="godotObject"/>.
    /// </summary>
    /// <remarks>The developer should only supply the <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</remarks>
    /// <param name="godotObject">The <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</param>
    /// <returns>The existing or a new instance of the <see cref="SpoutTexture"/> wrapper script attached to the supplied <paramref name="godotObject"/>.</returns>
    public new static SpoutTexture Bind(GodotObject godotObject)
    {
#if DEBUG
        if (!IsInstanceValid(godotObject))
            throw new InvalidOperationException("The supplied GodotObject instance is not valid.");
#endif
        if (godotObject is SpoutTexture wrapperScriptInstance)
            return wrapperScriptInstance;

#if DEBUG
        var expectedType = typeof(SpoutTexture);
        var currentObjectClassName = godotObject.GetClass();
        if (!ClassDB.IsParentClass(expectedType.Name, currentObjectClassName))
            throw new InvalidOperationException($"The supplied GodotObject ({currentObjectClassName}) is not the {expectedType.Name} type.");
#endif

        if (_wrapperScriptAsset is null)
        {
            var scriptPathAttribute = typeof(SpoutTexture).GetCustomAttributes<ScriptPathAttribute>().FirstOrDefault();
            if (scriptPathAttribute is null) throw new UnreachableException();
            _wrapperScriptAsset = ResourceLoader.Load<CSharpScript>(scriptPathAttribute.Path);
        }

        var instanceId = godotObject.GetInstanceId();
        godotObject.SetScript(_wrapperScriptAsset);
        return (SpoutTexture)InstanceFromId(instanceId);
    }

    /// <summary>
    /// Creates an instance of the GDExtension <see cref="SpoutTexture"/> type, and attaches a wrapper script instance to it.
    /// </summary>
    /// <returns>The wrapper instance linked to the underlying GDExtension "SpoutTexture" type.</returns>
    public new static SpoutTexture Instantiate() => Bind(ClassDB.Instantiate(NativeName).As<GodotObject>());

    public new static class GDExtensionPropertyName
    {
        public new static readonly StringName SenderName = "sender_name";
        public new static readonly StringName UpdateInEditor = "update_in_editor";
    }

    public new string SenderName
    {
        get => Get(GDExtensionPropertyName.SenderName).As<string>();
        set => Set(GDExtensionPropertyName.SenderName, value);
    }

    public new bool UpdateInEditor
    {
        get => Get(GDExtensionPropertyName.UpdateInEditor).As<bool>();
        set => Set(GDExtensionPropertyName.UpdateInEditor, value);
    }

}
