public class StateTree {
    private State Start { get; init; }

    private int Depth { get; init; }

    public StateTree(State start, int depth) {
        Start = start;
        Depth = depth;
    }

    public State BuildAndEvaluate() {
        //Todo: very primitive Solution
        return Start.GenerateNextStates().MaxBy(x => Start.WhiteToMove ? x.SingleEvaluate() : -x.SingleEvaluate())!;
    }
}