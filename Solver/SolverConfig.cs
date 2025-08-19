using Craftimizer.Simulator.Actions;
using System.Collections.Frozen;
using System.Runtime.InteropServices;

namespace Craftimizer.Solver;

public enum SolverAlgorithm
{
    Oneshot,
    OneshotForked,
    Stepwise,
    StepwiseForked,
    StepwiseGenetic,
    Raphael,
}

[StructLayout(LayoutKind.Auto)]
public readonly record struct SolverConfig
{
    // MCTS configuration
    public int Iterations { get; init; }
    public int MaxIterations { get; init; }
    public float MaxScoreWeightingConstant { get; init; }
    public float ExplorationConstant { get; init; }
    public int MaxStepCount { get; init; }
    public int MaxRolloutStepCount { get; init; }
    public int ForkCount { get; init; }
    public int FurcatedActionCount { get; init; }
    public bool StrictActions { get; init; }

    // MCTS score weights
    public float ScoreProgress { get; init; }
    public float ScoreQuality { get; init; }
    public float ScoreDurability { get; init; }
    public float ScoreCP { get; init; }
    public float ScoreSteps { get; init; }

    // Raphael/A* configuration
    public bool Adversarial { get; init; }
    public bool BackloadProgress { get; init; }

    public int MaxThreadCount { get; init; }
    public ActionType[] ActionPool { get; init; }
    public SolverAlgorithm Algorithm { get; init; }

    public SolverConfig()
    {
        Iterations = 100_000;
        MaxIterations = 1_500_000;
        MaxScoreWeightingConstant = 0.1f;
        ExplorationConstant = 4;
        MaxStepCount = 30;
        MaxRolloutStepCount = 99;
        // Use 75% of all cores if less than 12 cores are available, otherwise use all but 4 cores. Keep at least 1 core.
        MaxThreadCount = Math.Max(1, Math.Max(Environment.ProcessorCount - 4, (int)MathF.Floor(Environment.ProcessorCount * 0.75f)));
        // Use 32 forks at minimum, or the number of cores, whichever is higher.
        ForkCount = Math.Max(Environment.ProcessorCount, 32);
        FurcatedActionCount = ForkCount / 2;
        StrictActions = true;

        ScoreProgress = 10;
        ScoreQuality = 80;
        ScoreDurability = 2;
        ScoreCP = 3;
        ScoreSteps = 5;

        ActionPool = DeterministicActionPool;
        Algorithm = SolverAlgorithm.StepwiseGenetic;
    }

    public static ActionType[] OptimizeActionPool(IEnumerable<ActionType> actions) =>
        [.. actions.Order()];

    public SolverConfig FilterSpecialistActions() =>
        this with { ActionPool = ActionPool.Where(action => !SpecialistActions.Contains(action)).ToArray() };

    public static readonly ActionType[] DeterministicActionPool = OptimizeActionPool(new[]
    {
        ActionType.MuscleMemory,
        ActionType.Reflect,
        ActionType.TrainedEye,

        ActionType.BasicSynthesis,
        ActionType.CarefulSynthesis,
        ActionType.Groundwork,
        ActionType.DelicateSynthesis,
        ActionType.PrudentSynthesis,

        ActionType.BasicTouch,
        ActionType.StandardTouch,
        ActionType.ByregotsBlessing,
        ActionType.PrudentTouch,
        ActionType.AdvancedTouch,
        ActionType.PreparatoryTouch,
        ActionType.TrainedFinesse,
        ActionType.RefinedTouch,

        ActionType.MastersMend,
        ActionType.WasteNot,
        ActionType.WasteNot2,
        ActionType.Manipulation,
        ActionType.ImmaculateMend,
        ActionType.TrainedPerfection,

        ActionType.Veneration,
        ActionType.GreatStrides,
        ActionType.Innovation,
        ActionType.QuickInnovation,

        ActionType.Observe,
        ActionType.HeartAndSoul,

        ActionType.StandardTouchCombo,
        ActionType.AdvancedTouchCombo,
        ActionType.ObservedAdvancedTouchCombo,
        ActionType.RefinedTouchCombo,
    });

    // Same as deterministic, but with condition-specific actions added
    public static readonly ActionType[] RandomizedActionPool = OptimizeActionPool(new[]
    {
        ActionType.MuscleMemory,
        ActionType.Reflect,
        ActionType.TrainedEye,

        ActionType.BasicSynthesis,
        ActionType.CarefulSynthesis,
        ActionType.Groundwork,
        ActionType.DelicateSynthesis,
        ActionType.IntensiveSynthesis,
        ActionType.PrudentSynthesis,

        ActionType.BasicTouch,
        ActionType.StandardTouch,
        ActionType.ByregotsBlessing,
        ActionType.PreciseTouch,
        ActionType.PrudentTouch,
        ActionType.AdvancedTouch,
        ActionType.PreparatoryTouch,
        ActionType.TrainedFinesse,
        ActionType.RefinedTouch,

        ActionType.MastersMend,
        ActionType.WasteNot,
        ActionType.WasteNot2,
        ActionType.Manipulation,
        ActionType.ImmaculateMend,
        ActionType.TrainedPerfection,

        ActionType.Veneration,
        ActionType.GreatStrides,
        ActionType.Innovation,
        ActionType.QuickInnovation,

        ActionType.Observe,
        ActionType.HeartAndSoul,
        ActionType.TricksOfTheTrade,

        ActionType.StandardTouchCombo,
        ActionType.AdvancedTouchCombo,
        ActionType.ObservedAdvancedTouchCombo,
        ActionType.RefinedTouchCombo,
    });

    public static readonly FrozenSet<ActionType> InefficientActions =
        new[]
        {
            ActionType.CarefulObservation,
            ActionType.FinalAppraisal
        }.ToFrozenSet();

    public static readonly FrozenSet<ActionType> RiskyActions =
        new[]
        {
            ActionType.RapidSynthesis,
            ActionType.HastyTouch,
            ActionType.DaringTouch,
        }.ToFrozenSet();

    public static readonly FrozenSet<ActionType> SpecialistActions =
        new[]
        {
            ActionType.CarefulObservation,
            ActionType.HeartAndSoul,
            ActionType.QuickInnovation,
        }.ToFrozenSet();

    public static readonly SolverConfig RecipeNoteDefault = new SolverConfig() with
    {

    };

    public static readonly SolverConfig EditorDefault = new SolverConfig() with
    {
        Algorithm = SolverAlgorithm.Raphael,
        Adversarial = true
    };

    public static readonly SolverConfig SynthHelperDefault = new SolverConfig() with
    {
        ActionPool = RandomizedActionPool
    };
}
