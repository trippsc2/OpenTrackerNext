using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Reactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTrackerNext.Core.Entities;
using OpenTrackerNext.Core.Equality;
using OpenTrackerNext.Core.Icons.Counted;
using OpenTrackerNext.Core.Icons.Labels;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.Json;
using Xunit;

namespace OpenTrackerNext.Core.UnitTests.Icons.Counted;

[ExcludeFromCodeCoverage]
public sealed class CountedIconPrototypeTests
{
    private readonly CountedIconPrototype _subject = new();
    
    [Fact]
    public void DocumentChanges_ShouldEmitWhenAnyPropertyChanges()
    {
        using var observer = _subject.DocumentChanges.Observe();
        
        _subject.Entity = EntityId.New();
        _subject.DefaultImage = ImageId.New();
        _subject.DisabledImage = ImageId.New();
        _subject.LabelPosition = IconLabelPosition.TopLeft;
        _subject.HideLabelAtMinimum = false;
        _subject.AddAsteriskAtMaximum = false;
        _subject.IndeterminateState = IndeterminateState.Maximum;
        _subject.SwapClickActions = true;
        _subject.CycleEntityCounts = true;

        observer.Should().Push(9);
    }
    
    [Fact]
    public void Entity_ShouldInitializeAsEmptyEntityId()
    {
        _subject.Entity
            .Should()
            .Be(EntityId.Empty);
    }
    
    [Fact]
    public void Entity_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.Entity = EntityId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.Entity);
    }
    
    [Fact]
    public void Entity_ShouldSerializeAsExpected()
    {
        var entity = EntityId.New();
        _subject.Entity = entity;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.Entity)]?
            .Value<string>()
            .Should()
            .Be(entity.Value.ToString());
    }
    
    [Fact]
    public void Entity_ShouldDeserializeAsExpected()
    {
        var entity = EntityId.New();
        
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.Entity), new JValue(entity.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.Entity
            .Should()
            .Be(entity);
    }
    
    [Fact]
    public void DefaultImage_ShouldInitializeAsEmptyImageId()
    {
        _subject.DefaultImage
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void DefaultImage_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.DefaultImage = ImageId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.DefaultImage);
    }
    
    [Fact]
    public void DefaultImage_ShouldSerializeAsExpected()
    {
        var image = ImageId.New();
        _subject.DefaultImage = image;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.DefaultImage)]?
            .Value<string>()
            .Should()
            .Be(image.Value.ToString());
    }
    
    [Fact]
    public void DefaultImage_ShouldDeserializeAsExpected()
    {
        var image = ImageId.New();
        
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.DefaultImage), new JValue(image.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.DefaultImage
            .Should()
            .Be(image);
    }
    
    [Fact]
    public void DisabledImage_ShouldInitializeAsNullImageId()
    {
        _subject.DisabledImage
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void DisabledImage_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.DisabledImage = ImageId.New();
        
        monitor.Should().RaisePropertyChangeFor(x => x.DisabledImage);
    }
    
    [Fact]
    public void DisabledImage_ShouldSerializeAsExpected_WhenHasDisabledImage()
    {
        var image = ImageId.New();
        _subject.DisabledImage = image;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.DisabledImage)]?
            .Value<string>()
            .Should()
            .Be(image.Value.ToString());
    }
    
    [Fact]
    public void DisabledImage_ShouldDeserializeAsExpected_WhenHasDisabledImage()
    {
        var image = ImageId.New();
        
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.DisabledImage), new JValue(image.Value.ToString()) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.DisabledImage
            .Should()
            .Be(image);
    }
    
    [Fact]
    public void DisabledImage_ShouldDeserializeAsExpected_WhenDoesNotHaveDisabledImage()
    {
        var jsonObject = new JObject();
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        JsonContext.Deserialize<CountedIconPrototype>(json);

        _subject.DisabledImage
            .Should()
            .Be(ImageId.Empty);
    }
    
    [Fact]
    public void LabelPosition_ShouldInitializeAsBottomRight()
    {
        _subject.LabelPosition
            .Should()
            .Be(IconLabelPosition.BottomRight);
    }
    
    [Fact]
    public void LabelPosition_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.LabelPosition = IconLabelPosition.TopLeft;
        
        monitor.Should().RaisePropertyChangeFor(x => x.LabelPosition);
    }
    
    [Fact]
    public void LabelPosition_ShouldSerializeAsExpected()
    {
        var position = IconLabelPosition.TopLeft;
        _subject.LabelPosition = position;

        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.LabelPosition)]?
            .Value<string>()
            .Should()
            .Be(position.Name);
    }
    
    [Fact]
    public void LabelPosition_ShouldDeserializeAsExpected()
    {
        var position = IconLabelPosition.TopLeft;
        
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.LabelPosition), new JValue(position.Name) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.LabelPosition
            .Should()
            .Be(position);
    }
    
    [Fact]
    public void HideLabelAtMinimum_ShouldInitializeAsTrue()
    {
        _subject.HideLabelAtMinimum
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void HideLabelAtMinimum_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.HideLabelAtMinimum = false;
        
        monitor.Should().RaisePropertyChangeFor(x => x.HideLabelAtMinimum);
    }
    
    [Fact]
    public void HideLabelAtMinimum_ShouldSerializeAsExpected()
    {
        _subject.HideLabelAtMinimum = false;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.HideLabelAtMinimum)]?
            .Value<bool>()
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void HideLabelAtMinimum_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.HideLabelAtMinimum), new JValue(false) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.HideLabelAtMinimum
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void AddAsteriskAtMaximum_ShouldInitializeAsTrue()
    {
        _subject.AddAsteriskAtMaximum
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void AddAsteriskAtMaximum_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.AddAsteriskAtMaximum = false;
        
        monitor.Should().RaisePropertyChangeFor(x => x.AddAsteriskAtMaximum);
    }
    
    [Fact]
    public void AddAsteriskAtMaximum_ShouldSerializeAsExpected()
    {
        _subject.AddAsteriskAtMaximum = false;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.AddAsteriskAtMaximum)]?
            .Value<bool>()
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void AddAsteriskAtMaximum_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.AddAsteriskAtMaximum), new JValue(false) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.AddAsteriskAtMaximum
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void IndeterminateState_ShouldInitializeAsNone()
    {
        _subject.IndeterminateState
            .Should()
            .Be(IndeterminateState.None);
    }
    
    [Fact]
    public void HasIndeterminateState_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.IndeterminateState = IndeterminateState.Minimum;
        
        monitor.Should().RaisePropertyChangeFor(x => x.IndeterminateState);
    }
    
    [Theory]
    [InlineData(nameof(IndeterminateState.None))]
    [InlineData(nameof(IndeterminateState.Minimum))]
    [InlineData(nameof(IndeterminateState.Maximum))]
    public void HasIndeterminateState_ShouldSerializeAsExpected(string expectedName)
    {
        var indeterminateState = IndeterminateState.FromName(expectedName);
        _subject.IndeterminateState = indeterminateState;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.IndeterminateState)]?
            .Value<string>()
            .Should()
            .Be(expectedName);
    }
    
    [Theory]
    [InlineData(nameof(IndeterminateState.None))]
    [InlineData(nameof(IndeterminateState.Minimum))]
    [InlineData(nameof(IndeterminateState.Maximum))]
    public void HasIndeterminateState_ShouldDeserializeAsExpected(string expectedName)
    {
        var expected = IndeterminateState.FromName(expectedName);
        
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.IndeterminateState), new JValue(expectedName) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.IndeterminateState
            .Should()
            .Be(expected);
    }
    
    [Fact]
    public void SwapClickActions_ShouldInitializeAsFalse()
    {
        _subject.SwapClickActions
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void SwapClickActions_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.SwapClickActions = true;
        
        monitor.Should().RaisePropertyChangeFor(x => x.SwapClickActions);
    }
    
    [Fact]
    public void SwapClickActions_ShouldSerializeAsExpected()
    {
        _subject.SwapClickActions = true;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.SwapClickActions)]?
            .Value<bool>()
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void SwapClickActions_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.SwapClickActions), new JValue(true) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.SwapClickActions
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldInitializeAsFalse()
    {
        _subject.CycleEntityCounts
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldRaisePropertyChanged_WhenChanged()
    {
        using var monitor = _subject.Monitor();
        
        _subject.CycleEntityCounts = true;
        
        monitor.Should().RaisePropertyChangeFor(x => x.CycleEntityCounts);
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldSerializeAsExpected()
    {
        _subject.CycleEntityCounts = true;
        
        var json = JsonContext.Serialize(_subject);
        var jsonObject = JObject.Parse(json);

        jsonObject[nameof(CountedIconPrototype.CycleEntityCounts)]?
            .Value<bool>()
            .Should()
            .BeTrue();
    }
    
    [Fact]
    public void CycleEntityCounts_ShouldDeserializeAsExpected()
    {
        var jsonObject = new JObject
        {
            { nameof(CountedIconPrototype.CycleEntityCounts), new JValue(true) }
        };
        
        var json = jsonObject.ToString(Formatting.Indented);
        
        var subject = JsonContext.Deserialize<CountedIconPrototype>(json);

        subject.CycleEntityCounts
            .Should()
            .BeTrue();
    }

    [Fact]
    public void MakeValueEqualTo_ShouldDoNothing_WhenOtherIsReferenceEqual()
    {
        ((IMakeValueEqual) _subject).MakeValueEqualTo(_subject);
    }

    [Fact]
    public void MakeValueEqualTo_ShouldSetPropertiesValueEqual_WhenOtherIsSameType()
    {
        _subject.Entity = EntityId.New();
        _subject.DefaultImage = ImageId.New();
        _subject.DisabledImage = ImageId.New();
        _subject.LabelPosition = IconLabelPosition.BottomRight;
        _subject.HideLabelAtMinimum = true;
        _subject.AddAsteriskAtMaximum = true;
        _subject.IndeterminateState = IndeterminateState.None;
        _subject.SwapClickActions = false;
        _subject.CycleEntityCounts = false;

        var other = new CountedIconPrototype
        {
            Entity = EntityId.New(),
            DefaultImage = ImageId.New(),
            DisabledImage = ImageId.New(),
            LabelPosition = IconLabelPosition.TopLeft,
            HideLabelAtMinimum = false,
            AddAsteriskAtMaximum = false,
            IndeterminateState = IndeterminateState.Maximum,
            SwapClickActions = true,
            CycleEntityCounts = true
        };

        ((IMakeValueEqual) _subject).MakeValueEqualTo(other);
        
        _subject.Entity
            .Should()
            .Be(other.Entity);
        _subject.DefaultImage
            .Should()
            .Be(other.DefaultImage);
        _subject.DisabledImage
            .Should()
            .Be(other.DisabledImage);
        _subject.LabelPosition
            .Should()
            .Be(other.LabelPosition);
        _subject.HideLabelAtMinimum
            .Should()
            .Be(other.HideLabelAtMinimum);
        _subject.AddAsteriskAtMaximum
            .Should()
            .Be(other.AddAsteriskAtMaximum);
        _subject.IndeterminateState
            .Should()
            .Be(other.IndeterminateState);
        _subject.SwapClickActions
            .Should()
            .Be(other.SwapClickActions);
        _subject.CycleEntityCounts
            .Should()
            .Be(other.CycleEntityCounts);
    }

    [Fact]
    public void MakeValueEqualTo_ShouldReturnFalseAndDoNothing_WhenOtherIsDifferentType()
    {
        ((IMakeValueEqual) _subject).MakeValueEqualTo(new object())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Clone_ShouldReturnNewEquivalentInstance()
    {
        var clone = ((ICloneable)_subject).Clone();

        clone
            .Should()
            .BeOfType<CountedIconPrototype>()
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
    public void ValueEquals_ShouldReturnTrue_WhenOtherIsSameTypeAndPropertyValuesAreEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;
        
        _subject.Entity = entity;
        _subject.DefaultImage = defaultImage;
        _subject.DisabledImage = disabledImage;
        _subject.LabelPosition = labelPosition;
        _subject.HideLabelAtMinimum = hideLabelAtMinimum;
        _subject.AddAsteriskAtMaximum = addAsteriskAtMaximum;
        _subject.IndeterminateState = indeterminateState;
        _subject.SwapClickActions = swapClickActions;
        _subject.CycleEntityCounts = cycleEntityCounts;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndEntityValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        _subject.Entity = EntityId.New();
        _subject.DefaultImage = defaultImage;
        _subject.DisabledImage = disabledImage;
        _subject.LabelPosition = labelPosition;
        _subject.HideLabelAtMinimum = hideLabelAtMinimum;
        _subject.AddAsteriskAtMaximum = addAsteriskAtMaximum;
        _subject.IndeterminateState = indeterminateState;
        _subject.SwapClickActions = swapClickActions;
        _subject.CycleEntityCounts = cycleEntityCounts;
        
        var other = new CountedIconPrototype
        {
            Entity = EntityId.New(),
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndDefaultImageValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;

        var entity = EntityId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        _subject.Entity = entity;
        _subject.DefaultImage = ImageId.New();
        _subject.DisabledImage = disabledImage;
        _subject.LabelPosition = labelPosition;
        _subject.HideLabelAtMinimum = hideLabelAtMinimum;
        _subject.AddAsteriskAtMaximum = addAsteriskAtMaximum;
        _subject.IndeterminateState = indeterminateState;
        _subject.SwapClickActions = swapClickActions;
        _subject.CycleEntityCounts = cycleEntityCounts;
        
        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = ImageId.New(),
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndHasDisabledImageAndDisabledImageValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        _subject.Entity = entity;
        _subject.DefaultImage = defaultImage;
        _subject.DisabledImage = ImageId.New();
        _subject.LabelPosition = labelPosition;
        _subject.HideLabelAtMinimum = hideLabelAtMinimum;
        _subject.AddAsteriskAtMaximum = addAsteriskAtMaximum;
        _subject.IndeterminateState = indeterminateState;
        _subject.SwapClickActions = swapClickActions;
        _subject.CycleEntityCounts = cycleEntityCounts;
        
        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = ImageId.New(),
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndLabelPositionValuesAreNotEqual()
    {
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = true,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndHideLabelAtMinimumValuesAreNotEqual()
    {
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = true,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable) _subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndAddAsteriskAtMaximumValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = true,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndHasIndeterminateStateValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = IndeterminateState.Maximum,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }
    
    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndSwapClickActionsValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool cycleEntityCounts = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = true,
            CycleEntityCounts = cycleEntityCounts
        };

        ((IValueEquatable)_subject)
            .ValueEquals(other)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ValueEquals_ShouldReturnFalse_WhenOtherIsSameTypeAndCycleEntityCountsValuesAreNotEqual()
    {
        const bool hideLabelAtMinimum = false;
        const bool addAsteriskAtMaximum = false;
        const bool swapClickActions = true;
        
        var entity = EntityId.New();
        var defaultImage = ImageId.New();
        var disabledImage = ImageId.New();
        var labelPosition = IconLabelPosition.TopLeft;
        var indeterminateState = IndeterminateState.Maximum;

        var other = new CountedIconPrototype
        {
            Entity = entity,
            DefaultImage = defaultImage,
            DisabledImage = disabledImage,
            LabelPosition = labelPosition,
            HideLabelAtMinimum = hideLabelAtMinimum,
            AddAsteriskAtMaximum = addAsteriskAtMaximum,
            IndeterminateState = indeterminateState,
            SwapClickActions = swapClickActions,
            CycleEntityCounts = true
        };

        ((IValueEquatable)_subject).ValueEquals(other)
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