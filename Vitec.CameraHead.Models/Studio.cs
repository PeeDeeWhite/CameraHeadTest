namespace Vitec.CameraHead.Models;

using System.Collections.Generic;
using System.Linq;

/// <summary>
///     Represents a Studio, a space containing a number of camera heads <see cref="ICameraHead" /> to test
///     and a number of targets <see cref="Target" /> for them to aim at
/// </summary>
/// <remarks>
///     Reverse engineered from Interview1 namespace
/// </remarks>
public sealed class Studio(IEnumerable<Target> targets, IEnumerable<ICameraHead> cameraHeads)
{
    private readonly List<ICameraHead> _cameraHeads = cameraHeads == null ? new List<ICameraHead>() : cameraHeads.ToList();

    private readonly List<Target> _targets = targets == null ? new List<Target>() : targets.ToList();

    public IEnumerable<ICameraHead> CameraHeads => _cameraHeads;

    public IEnumerable<Target> Targets => _targets;
}