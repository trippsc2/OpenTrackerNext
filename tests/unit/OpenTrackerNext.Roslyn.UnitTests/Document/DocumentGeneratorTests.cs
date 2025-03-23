using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using OpenTrackerNext.Roslyn.Document;
using Xunit;

namespace OpenTrackerNext.Roslyn.UnitTests.Document;

[ExcludeFromCodeCoverage]
public sealed class DocumentGeneratorTests
{
    // lang=cs
    private const string InterfacesSourceCode =
        """
        namespace OpenTrackerNext.Core.Documents
        {
            /// <summary>
            /// Represents the content data of a document.
            /// </summary>
            public interface IDocumentData : global::System.ICloneable, IMakeValueEqual, IValueEquatable
            {
                /// <summary>
                /// Gets an <see cref="IObservable{T}"/> representing changes made to the document or any of its children.
                /// </summary>
                global::System.IObservable<global::System.Reactive.Unit> DocumentChanges { get; }
            }
            
            /// <summary>
            /// Represents the content data of a document.
            /// </summary>
            /// <typeparam name="TSelf">
            /// The type of document data.
            /// </typeparam>
            public interface IDocumentData<TSelf> : IDocumentData, ICloneable<TSelf>, IMakeValueEqual<TSelf>, IValueEquatable<TSelf>
                where TSelf : IDocumentData<TSelf>, new()
            {
            }
        }
        
        namespace OpenTrackerNext.Core.Equality
        {
            /// <summary>
            /// Represents an object that can be cloned.
            /// </summary>
            /// <typeparam name="TSelf">
            /// The type of the object that implements this interface.
            /// </typeparam>
            public interface ICloneable<out TSelf> : global::System.ICloneable
                where TSelf : ICloneable<TSelf>
            {
                /// <summary>
                /// Returns a new <typeparamref name="TSelf"/> instance that is value equal to this instance. 
                /// </summary>
                /// <returns>
                /// A new <typeparamref name="TSelf"/> instance that is value equal to this instance.
                /// </returns>
                new TSelf Clone();
            }
            
            /// <summary>
            /// Represents data that can be made value equal to another object.
            /// </summary>
            public interface IMakeValueEqual
            {
                /// <summary>
                /// Makes this object value equal to another object.
                /// </summary>
                /// <param name="other">
                /// An <see cref="object"/> to which this object will be made value equal.
                /// </param>
                /// <returns>
                /// A <see cref="bool"/> representing whether the object was made value equal.
                /// </returns>
                bool MakeValueEqualTo(object other);
            }

            /// <summary>
            /// Represents data that can be made value equal to another object.
            /// </summary>
            /// <typeparam name="TSelf">
            /// The type to which this object can be made value equal.
            /// </typeparam>
            public interface IMakeValueEqual<in TSelf> : IMakeValueEqual
                where TSelf : IMakeValueEqual<TSelf>, new()
            {
                /// <summary>
                /// Makes this object value equal to the specified object.
                /// </summary>
                /// <param name="other">
                /// A <typeparamref name="TSelf"/> to which this object will be made value equal.
                /// </param>
                void MakeValueEqualTo(TSelf other);
            }
            
            /// <summary>
            /// Represents a class that can be compared for value Document.
            /// </summary>
            public interface IValueEquatable
            {
                /// <summary>
                /// Returns a value indicating whether this instance is equal to a specified object.
                /// </summary>
                /// <param name="obj">
                /// An <see cref="object"/> to compare with this instance.
                /// </param>
                /// <returns>
                /// A <see cref="bool"/> representing whether this instance is equal to the specified object.
                /// </returns>
                bool ValueEquals(object? obj);
            }
            
            /// <summary>
            /// Represents a class that can be compared for value Document.
            /// </summary>
            /// <typeparam name="TSelf">
            /// The type of the class that implements this interface.
            /// </typeparam>
            public interface IValueEquatable<in TSelf> : IValueEquatable
                where TSelf : IValueEquatable<TSelf>
            {
                /// <summary>
                /// Returns whether the object is value equal to the specified object.
                /// </summary>
                /// <param name="other">
                /// A <typeparamref name="TSelf"/> representing the object to compare with this object.
                /// </param>
                /// <returns>
                /// A <see cref="bool"/> representing whether the object is value equal to the specified object.
                /// </returns>
                bool ValueEquals(TSelf other);
            }
        }
        """;
    
    [Fact]
    public async Task ShouldDoNothing_WhenNoMatchingCodeIsFound()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable
            using OpenTrackerNext.Core.Equality;
            
            {{InterfacesSourceCode}}
            
            namespace TestProject
            {
                public partial class NotMatchingTest : IMakeValueEqual<NotMatchingTest>
                {
                    public void MakeValueEqualTo(NotMatchingTest other)
                    {
                    }
                    
                    bool IMakeValueEqual.MakeValueEqualTo(object other)
                    {
                        if (other is not NotMatchingTest otherTest)
                        {
                            return false;
                        }
                        
                        MakeValueEqualTo(otherTest);
                        return true;
                    }
                }
            }
            """;
        
        await GeneratorTest<DocumentGenerator>
            .VerifyGeneratorAsync(
                source,
                (DocumentGenerator.DocumentAttributeHintName, DocumentGenerator.DocumentAttributeSource),
                (DocumentGenerator.DocumentMemberAttributeHintName, DocumentGenerator.DocumentMemberAttributeSource),
                (DocumentGenerator.ObservableCollectionExtensionsHintName, DocumentGenerator.ObservableCollectionExtensionsSource));
    }

    [Fact]
    public async Task ShouldGenerateExpected_WhenMatchingCodeHasValueTypeProperty()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable
            using OpenTrackerNext.Core.Equality;
            using OpenTrackerNext.Document;
            using ReactiveUI;
            
            {{InterfacesSourceCode}}
            
            namespace TestProject.Generated
            {
            }
            
            namespace TestProject
            {
                [Document]
                public partial class TestObject : ReactiveObject
                {
                    [DocumentMember]
                    public int Value { get; set; }
                }
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable
            
            using OpenTrackerNext.Document;
            
            namespace TestProject;
            
            partial class TestObject : global::OpenTrackerNext.Core.Documents.IDocumentData<TestProject.TestObject>
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="TestObject"/> class.
                /// </summary>
                public TestObject()
                {
                    DocumentChanges = global::System.Reactive.Linq.Observable.Select(Changed, _ => global::System.Reactive.Unit.Default);
                }
                
                /// <inheritdoc/>
                public global::System.IObservable<global::System.Reactive.Unit> DocumentChanges { get; }
                
                public void MakeValueEqualTo(TestObject other)
                {
                    if (ReferenceEquals(this, other))
                    {
                        return;
                    }
            
                    this.Value = other.Value;
                }
            
                bool global::OpenTrackerNext.Core.Equality.IMakeValueEqual.MakeValueEqualTo(object other)
                {
                    if (other is not TestObject otherData)
                    {
                        return false;
                    }
            
                    MakeValueEqualTo(otherData);
                    return true;
                }
                
                public TestObject Clone()
                {
                    var clone = new TestObject();
                    
                    clone.MakeValueEqualTo(this);
                    
                    return clone;
                }
                
                object global::System.ICloneable.Clone()
                {
                    return Clone();
                }
                
                public bool ValueEquals(TestObject other)
                {
                    return this.Value.Equals(other.Value);
                }
                
                bool global::OpenTrackerNext.Core.Equality.IValueEquatable.ValueEquals(object? other)
                {
                    if (other is null)
                    {
                        return false;
                    }
                    
                    if (ReferenceEquals(this, other))
                    {
                        return true;
                    }
                    
                    if (other is not TestObject otherData)
                    {
                        return false;
                    }
            
                    return ValueEquals(otherData);
                }
            }
            """;
        
        await GeneratorTest<DocumentGenerator>
            .VerifyGeneratorAsync(
                source,
                (DocumentGenerator.DocumentAttributeHintName, DocumentGenerator.DocumentAttributeSource),
                (DocumentGenerator.DocumentMemberAttributeHintName, DocumentGenerator.DocumentMemberAttributeSource),
                (DocumentGenerator.ObservableCollectionExtensionsHintName, DocumentGenerator.ObservableCollectionExtensionsSource),
                ("TestProject.TestObject.g.cs", expected));
    }

    [Fact]
    public async Task ShouldGenerateExpected_WhenMatchingCodeHasKnownImmutableReferenceTypeProperty()
    {
        // lang=cs
        const string source =
            $$"""
            #nullable enable
            
            using System;
            using OpenTrackerNext.Core.Equality;
            using OpenTrackerNext.Document;
            using ReactiveUI;
            
            {{InterfacesSourceCode}}
            
            namespace TestProject.Generated
            {
            }
            
            namespace TestProject
            {
                [Document]
                public partial class TestObject : ReactiveObject
                {
                    [DocumentMember]
                    public Version Value { get; set; } = new Version(1, 0, 0, 0);
                }
            }
            """;
        
        // lang=cs
        const string expected =
            """
            // <auto-generated/>
            #nullable enable
            
            using OpenTrackerNext.Document;
            
            namespace TestProject;
            
            partial class TestObject : global::OpenTrackerNext.Core.Documents.IDocumentData<TestProject.TestObject>
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="TestObject"/> class.
                /// </summary>
                public TestObject()
                {
                    DocumentChanges = global::System.Reactive.Linq.Observable.Select(Changed, _ => global::System.Reactive.Unit.Default);
                }
                
                /// <inheritdoc/>
                public global::System.IObservable<global::System.Reactive.Unit> DocumentChanges { get; }
                
                public void MakeValueEqualTo(TestObject other)
                {
                    if (ReferenceEquals(this, other))
                    {
                        return;
                    }
            
                    this.Value = other.Value;
                }
            
                bool global::OpenTrackerNext.Core.Equality.IMakeValueEqual.MakeValueEqualTo(object other)
                {
                    if (other is not TestObject otherData)
                    {
                        return false;
                    }
            
                    MakeValueEqualTo(otherData);
                    return true;
                }
                
                public TestObject Clone()
                {
                    var clone = new TestObject();
                    
                    clone.MakeValueEqualTo(this);
                    
                    return clone;
                }
                
                object global::System.ICloneable.Clone()
                {
                    return Clone();
                }
                
                public bool ValueEquals(TestObject other)
                {
                    return this.Value.Equals(other.Value);
                }
                
                bool global::OpenTrackerNext.Core.Equality.IValueEquatable.ValueEquals(object? other)
                {
                    if (other is null)
                    {
                        return false;
                    }
                    
                    if (ReferenceEquals(this, other))
                    {
                        return true;
                    }
                    
                    if (other is not TestObject otherData)
                    {
                        return false;
                    }
            
                    return ValueEquals(otherData);
                }
            }
            """;
        
        await GeneratorTest<DocumentGenerator>
            .VerifyGeneratorAsync(
                source,
                (DocumentGenerator.DocumentAttributeHintName, DocumentGenerator.DocumentAttributeSource),
                (DocumentGenerator.DocumentMemberAttributeHintName, DocumentGenerator.DocumentMemberAttributeSource),
                (DocumentGenerator.ObservableCollectionExtensionsHintName, DocumentGenerator.ObservableCollectionExtensionsSource),
                ("TestProject.TestObject.g.cs", expected));
    }
}