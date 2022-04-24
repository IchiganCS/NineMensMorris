public class StateTree {
    private State Start { get; init; }

    private int Depth { get; init; }

    public StateTree(State start, int depth) {
        Start = start;
        Depth = depth;
    }

    public State BuildAndEvaluate() {
        if (Depth == -1) {
            return Start.GenerateNextStates().MaxBy(x => Start.WhiteToMove ? x.SingleEvaluate() : -x.SingleEvaluate())!;
        }
        else
            return Start.GenerateNextStates().MaxBy(x => Start.WhiteToMove ? new StateTree(x, Depth - 1).BuildAndEvaluate().SingleEvaluate() : 
                -new StateTree(x, Depth - 1).BuildAndEvaluate().SingleEvaluate())!;
    }
}