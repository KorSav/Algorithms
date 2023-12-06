using System.Diagnostics;
using SearchInSetOfStates.NonInformative;
using SearchInSetOfStates.Informative;
using NQueensProblem;
using NQueensProblem.Visualization;
using SearchInSetOfStates;

const int MemoryLimitMBs = 1024;
const int TimeLimitMinutes = 15;

State stateStart;
Node? nodeResult, nodeTmp;
SearchAlgorithm? algoSearch;
Stopwatch stopwatch;
string elapsedTime, fileNameOutput, prefix;
bool timeLimitExceeded;
int indAlgo;
string[] directories = new string[] 
{ "AStar Search", "Iterative Deeping Search"};
FileStream fileStream;
StreamWriter writer;
Experiment curExp;
Experiment[] experiments = new Experiment[] {
        //new (8 , false), new (8 , true ), new (8 , true ), new (8 , true ),
        new (8 , true ), new (8 , true ), new (8 , true ), new (8 , true ),
        //new (9 , true ), new (9 , true ), new (10, true ), new (10, true ),
        //new (10, true ), new (12, true ), new (12, true ), new (12, true ),
        //new (14, true ), new (14, true ), new (14, true ), new (16, true ),
        //new (16, true ), new (16, true ), new (18, true ), new (18, true ),
        //new (18, true ), new (20, true ), new (20, true ), new (20, true ),
        //new (9 , false), new (10, false), new (11, false), new (12, false),
};

foreach ( var dirName in directories ) {
    if (Directory.Exists(dirName)) {
        Directory.Delete(dirName, true);
    }
    Directory.CreateDirectory(dirName);
}

for (int indExp = 0; indExp < experiments.Length; ++indExp ) {
    curExp = experiments[indExp];
    stateStart = new(curExp.StateSize);
    stateStart.PlaceDiagonal();
    if ( curExp.IsPlacementRandom ) {
        stateStart.Shuffle();
    }
    for ( indAlgo = 0; indAlgo < 2; indAlgo++ ) {

        algoSearch = null;
        GC.Collect();
        if ( indAlgo == 0 )
            algoSearch = new AStar(stateStart, Heuristic.F1);
        else
            algoSearch = new IterativeDeepingSearch(stateStart);
        algoSearch.MemoryLimitMBs = MemoryLimitMBs;
        nodeResult = null;
        stopwatch = new();
        CancellationTokenSource cts = new();
        Task t2 = Task.Run(() => {
            stopwatch.Start();  
            nodeResult = algoSearch.Search();
        }, cts.Token);
        timeLimitExceeded = ! t2.Wait(TimeSpan.FromMinutes(TimeLimitMinutes));
        stopwatch.Stop();
        if ( timeLimitExceeded) {
            cts.Cancel();
        }
        elapsedTime = $"{stopwatch.ElapsedMilliseconds * 1.0 / 1000}";
        fileNameOutput = $"{directories[indAlgo]}\\" +
            $"{indExp}_size{curExp.StateSize}.txt";
        fileStream =
            new(fileNameOutput, FileMode.Create, FileAccess.Write);
        writer = new(fileStream);

        if ( nodeResult == null ) {
            writer.WriteLine("[FAILED]");
            if ( timeLimitExceeded ) {
                writer.Write(
                    $"Time limit {TimeLimitMinutes} min - exceeded");
                Console.WriteLine(
                $"\nExperiment {indExp} - {directories[indAlgo]}:\n\t[FAILED TIME]");
            }
            if ( algoSearch.Info.MemoryOverused ) {
                writer.Write(
                    $"Memory limit {MemoryLimitMBs} megabytes - exceeded");
                Console.WriteLine(
                $"\nExperiment {indExp} - {directories[indAlgo]}:\n\t[FAILED MEMORY]");
            }
            writer.WriteLine();
            writer.WriteLine($"Search characteristics:\n{algoSearch.Info}");
            writer.WriteLine($"Elapsed time: {elapsedTime}");
            writer.Write($"(initial) State: {stateStart}");
            writer.WriteLine(StringConverter.Convert(stateStart));
            writer.Close();
            continue;
        }
        writer.WriteLine($"[SUCEED]\nResult node depth: {nodeResult.Depth}");
        writer.WriteLine($"Search characteristics:\n{algoSearch.Info}");
        writer.WriteLine($"Elapsed time: {elapsedTime}");

        nodeTmp = nodeResult;
        while ( nodeTmp != null ) {
            prefix = ( nodeTmp.Parent == null ) ? "(initial)" : "";
            writer.Write($"{prefix} State: {nodeTmp}");
            writer.WriteLine(StringConverter.Convert(nodeTmp, true));
            nodeTmp = nodeTmp.Parent;
        }
        writer.Close();
        Console.WriteLine(
                $"\nExperiment {indExp} - {directories[indAlgo]}:\n\t[SUCCEED]");
    }
}

class Experiment {
    public readonly int StateSize;
    public readonly bool IsPlacementRandom;
    public Experiment( int stateSize, bool isPlacementRandom ) {
        StateSize = stateSize;
        IsPlacementRandom = isPlacementRandom;
    }

}
