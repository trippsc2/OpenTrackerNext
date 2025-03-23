using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Json;
using OpenTrackerNext.Core.Metadata;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Metadata;

[ExcludeFromCodeCoverage]
public sealed class PackMetadataTests
{
    private readonly PackMetadata _subject = new();
    
    [Fact]
    public void TitlePrefix_ShouldReturnExpected()
    {
        PackMetadata.TitlePrefix
            .Should()
            .Be(PackMetadata.MetadataTitle);
    }
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Name = "test";
        _subject.Author = "test author";
        _subject.Version = new Version(1, 2, 3, 4);
        
        observer.Should().Push(3);
    }

    [Fact]
    public void Name_ShouldDefaultToEmptyString()
    {
        _subject.Name
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Name_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Name = "test";
        
        monitor.Should().RaisePropertyChangeFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void Name_ShouldSerializeAsExpected(string name)
    {
        _subject.Name = name;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(PackMetadata.Name)]?
            .Value<string>()
            .Should()
            .Be(name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void Name_ShouldDeserializeAsExpected(string name)
    {
        var jsonObject = new JObject
        {
            { nameof(PackMetadata.Name), new JValue(name) }
        };

        var json = jsonObject.ToString(Formatting.Indented); 

        var subject = JsonContext.Deserialize<PackMetadata>(json);

        subject.Name
            .Should()
            .Be(name);
    }

    [Fact]
    public void Author_ShouldDefaultToEmptyString()
    {
        _subject.Author
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Author_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Author = "test";
        
        monitor.Should().RaisePropertyChangeFor(x => x.Author);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void Author_ShouldSerializeAsExpected(string author)
    {
        _subject.Author = author;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(PackMetadata.Author)]?
            .Value<string>()
            .Should()
            .Be(author);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void Author_ShouldDeserializeAsExpected(string author)
    {
        var jsonObject = new JObject
        {
            { nameof(PackMetadata.Author), new JValue(author) }
        };

        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<PackMetadata>(json);

        subject.Author
            .Should()
            .Be(author);
    }

    [Fact]
    public void Version_ShouldInitializeToDefaultVersion()
    {
        _subject.Version
            .Should()
            .BeEquivalentTo(new Version(1, 0, 0, 0));
    }

    [Fact]
    public void Version_ShouldRaisePropertyChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Version = new Version(1, 2, 3, 4);
        
        monitor.Should().RaisePropertyChangeFor(x => x.Version);
    }

    [Fact]
    public void Version_ShouldSerializeAsExpected()
    {
        var version = new Version(1, 2, 3, 4);
        _subject.Version = version;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(PackMetadata.Version)]?
            .Value<string>()
            .Should()
            .Be(version.ToString());
    }

    [Fact]
    public void Version_ShouldDeserializeAsExpected()
    {
        var expected = new Version(1, 2, 3, 4);
        
        var jsonObject = new JObject
        {
            { nameof(PackMetadata.Version), new JValue(expected.ToString()) }
        };

        var json = jsonObject.ToString(Formatting.Indented);

        var subject = JsonContext.Deserialize<PackMetadata>(json);

        subject.Version
            .Should()
            .BeEquivalentTo(expected);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenReferenceEqual()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertiesValueEqual_WhenOtherIsSameType()
    {
        const string name = "test";
        const string author = "test author";
        const int major = 1;
        const int minor = 2;
        const int build = 3;
        const int revision = 4;
        
        var other = new PackMetadata
        {
            Name = name,
            Author = author,
            Version = new Version(major, minor, build, revision)
        };

        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(other)
            .Should()
            .BeTrue();
        
        _subject.Name
            .Should()
            .Be(name);
        
        _subject.Author
            .Should()
            .Be(author);
        
        _subject.Version
            .Major
            .Should()
            .Be(major);
        
        _subject.Version
            .Minor
            .Should()
            .Be(minor);
        
        _subject.Version
            .Build
            .Should()
            .Be(build);
        
        _subject.Version
            .Revision
            .Should()
            .Be(revision);
    }
    
    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual) _subject)
            .MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var clone = ((ICloneable)_subject).Clone();
        
        clone.Should()
            .BeOfType<PackMetadata>()
            .And.NotBeSameAs(_subject);

        ((IValueEquatable)_subject)
            .ValueEquals(clone)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsReferenceEqual()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(_subject)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameTypeAndPropertiesAreValueEqual()
    {
        const string name = "test";
        const string author = "test author";
        const int major = 1;
        const int minor = 2;
        const int build = 3;
        const int revision = 4;

        _subject.Name = name;
        _subject.Author = author;
        _subject.Version = new Version(major, minor, build, revision);
        
        var other = new PackMetadata
        {
            Name = name,
            Author = author,
            Version = new Version(major, minor, build, revision)
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndNamesAreNotEqual()
    {
        const string author = "test author";
        const int major = 1;
        const int minor = 2;
        const int build = 3;
        const int revision = 4;
        
        _subject.Name = "test";
        _subject.Author = author;
        _subject.Version = new Version(major, minor, build, revision);

        var other = new PackMetadata
        {
            Name = "other",
            Author = author,
            Version = new Version(major, minor, build, revision)
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndAuthorsAreNotEqual()
    {
        const string name = "test";
        const int major = 1;
        const int minor = 2;
        const int build = 3;
        const int revision = 4;

        _subject.Name = name;
        _subject.Author = "test author";
        _subject.Version = new Version(major, minor, build, revision);

        var other = new PackMetadata
        {
            Name = name,
            Author = "other",
            Version = new Version(major, minor, build, revision)
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndVersionsAreNotEqual()
    {
        const string name = "test";
        const string author = "test author";

        _subject.Name = name;
        _subject.Author = author;
        _subject.Version = new Version(1, 2, 3, 4);

        var other = new PackMetadata
        {
            Name = name,
            Author = author,
            Version = new Version(5, 6, 7, 8)
        };
        
        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsDifferentType()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsNull()
    {
        ((IValueEquatable)_subject)
            .ValueEquals(null)
            .Should()
            .BeFalse();
    }
}