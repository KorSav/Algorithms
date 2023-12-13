using System.Text;

namespace ClassicBee_GraphColoring
{
    internal class Program
    {
        static void Main( string[] args )
        {
            StreamWriter sw = new("data1.csv");
            GraphGenerator.Fill(out Graph g, 100, 1, 20);
            BeeSwarmHeuristic bsh = new(g, "log.txt", 30, 30, 1, 2);
            bsh.GeneratePlaces();
            for ( int i = 0; i < 1001; i++ ) {
                bsh.ChoosePlaces();
                bsh.SendFurages();
                //bsh.SendScouts();
                if ( i % 20 == 0 ) {
                    sw.WriteLine($"\"{i}\";" +
                        $"\"{bsh.Solution.ChromaticNumber}\"");
                }
            }
            bsh.SaveLogFile();
            sw.Close();
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Вихідний граф розмірністю {g.NodesDict.Count} " +
                $"було пофарбовано {bsh.Solution.ChromaticNumber} кольорами");
            Console.WriteLine($"Розфарбування:\n{bsh.Solution.ToStringPaints()}");
        }
    }
}
