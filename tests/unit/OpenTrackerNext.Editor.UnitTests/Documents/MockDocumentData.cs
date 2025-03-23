using System;
using System.Reactive;
using System.Reactive.Linq;
using OpenTrackerNext.Core.Documents;
using OpenTrackerNext.Core.Equality;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OpenTrackerNext.Editor.UnitTests.Documents;

public sealed class MockDocumentData : ReactiveObject, ITitledDocumentData<MockDocumentData>
{
    /// <summary>
    /// A <see cref="string"/> representing the title prefix for mock document data.
    /// </summary>
    public const string MockTitlePrefix = "Mock - ";

    /// <summary>
    /// Initializes a new <see cref="MockDocumentData"/> object.
    /// </summary>
    public MockDocumentData()
    {
        DocumentChanges = Changed.Select(_ => Unit.Default);
    }

    /// <inheritdoc />
    public static string TitlePrefix => MockTitlePrefix;
    
    /// <inheritdoc />
    public IObservable<Unit> DocumentChanges { get; }
    
    /// <summary>
    /// Gets or sets an <see cref="int"/> representing the value.
    /// </summary>
    [Reactive]
    public int Value { get; set; }

    /// <inheritdoc />
    public MockDocumentData Clone()
    {
        return new MockDocumentData
        {
            Value = Value
        };
    }

    /// <inheritdoc />
    public void MakeValueEqualTo(MockDocumentData other)
    {
        Value = other.Value;
    }

    /// <inheritdoc />
    public bool ValueEquals(MockDocumentData other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
        return Clone();
    }

    /// <inheritdoc />
    bool IMakeValueEqual.MakeValueEqualTo(object other)
    {
        if (other is not MockDocumentData otherData)
        {
            return false;
        }
        
        MakeValueEqualTo(otherData);
        return true;
    }

    /// <inheritdoc />
    bool IValueEquatable.ValueEquals(object? obj)
    {
        return obj is MockDocumentData otherData && ValueEquals(otherData);
    }
}