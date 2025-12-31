#pragma warning disable CS0109
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace GDExtension.Wrappers;

public partial class SpoutViewport : SubViewport
{

    private new static readonly StringName NativeName = new StringName("SpoutViewport");

    [Obsolete("Wrapper types cannot be constructed with constructors (it only instantiate the underlying SpoutViewport object), please use the Instantiate() method instead.")]
    protected SpoutViewport() { }

    private static CSharpScript _wrapperScriptAsset;

    /// <summary>
    /// Try to cast the script on the supplied <paramref name="godotObject"/> to the <see cref="SpoutViewport"/> wrapper type,
    /// if no script has attached to the type, or the script attached to the type does not inherit the <see cref="SpoutViewport"/> wrapper type,
    /// a new instance of the <see cref="SpoutViewport"/> wrapper script will get attaches to the <paramref name="godotObject"/>.
    /// </summary>
    /// <remarks>The developer should only supply the <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</remarks>
    /// <param name="godotObject">The <paramref name="godotObject"/> that represents the correct underlying GDExtension type.</param>
    /// <returns>The existing or a new instance of the <see cref="SpoutViewport"/> wrapper script attached to the supplied <paramref name="godotObject"/>.</returns>
    public new static SpoutViewport Bind(GodotObject godotObject)
    {
#if DEBUG
        if (!IsInstanceValid(godotObject))
            throw new InvalidOperationException("The supplied GodotObject instance is not valid.");
#endif
        if (godotObject is SpoutViewport wrapperScriptInstance)
            return wrapperScriptInstance;

#if DEBUG
        var expectedType = typeof(SpoutViewport);
        var currentObjectClassName = godotObject.GetClass();
        if (!ClassDB.IsParentClass(expectedType.Name, currentObjectClassName))
            throw new InvalidOperationException($"The supplied GodotObject ({currentObjectClassName}) is not the {expectedType.Name} type.");
#endif

        if (_wrapperScriptAsset is null)
        {
            var scriptPathAttribute = typeof(SpoutViewport).GetCustomAttributes<ScriptPathAttribute>().FirstOrDefault();
            if (scriptPathAttribute is null) throw new UnreachableException();
            _wrapperScriptAsset = ResourceLoader.Load<CSharpScript>(scriptPathAttribute.Path);
        }

        var instanceId = godotObject.GetInstanceId();
        godotObject.SetScript(_wrapperScriptAsset);
        return (SpoutViewport)InstanceFromId(instanceId);
    }

    /// <summary>
    /// Creates an instance of the GDExtension <see cref="SpoutViewport"/> type, and attaches a wrapper script instance to it.
    /// </summary>
    /// <returns>The wrapper instance linked to the underlying GDExtension "SpoutViewport" type.</returns>
    public new static SpoutViewport Instantiate() => Bind(ClassDB.Instantiate(NativeName).As<GodotObject>());

    public new static class GDExtensionPropertyName
    {
        public new static readonly StringName SenderName = "sender_name";
    }

    public new string SenderName
    {
        get => Get(GDExtensionPropertyName.SenderName).As<string>();
        set => Set(GDExtensionPropertyName.SenderName, value);
    }

}
