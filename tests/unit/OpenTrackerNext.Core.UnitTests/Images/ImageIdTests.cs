using System.Diagnostics.CodeAnalysis;
using OpenTrackerNext.Core.Images;
using OpenTrackerNext.Core.UnitTests.Ids;

namespace OpenTrackerNext.Core.UnitTests.Images;

[ExcludeFromCodeCoverage]
public sealed class ImageIdTests() : IdTests<ImageId>(ImageId.ImagesFolderName);