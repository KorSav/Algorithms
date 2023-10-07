using ProgramConfiguration;
using ExternalSortingAlgorithm;
using nmspcHelpFuncs;

/*
 * 50, 100, 3: 51 s
 * 50, 100, 5: 55 s
 * 50, 100, 7: 60 s
 * 512, 100, 3: 51 s
 * 30, 100, 3: 62 s
 * 30, 200, 3: 61 s 
 * 30, 100, 3: 30 s 
 * 512, 1024, 3: 2.53 min - 504 mb used/ sorted VVV
 * 30, 100, 3: 20.1 s 
 * 43.427s 42.505s 41.354s 30s(reverse)
 * 512, 1024, 3: 2 min (right order)
 * 512, 1024, 3: 2.35 min (reverse order)
 * 512, 1024, 3: 2.46 min (random order)
 */

ExternalSort externalSort = new ExternalSort();
HelpFuncs helpFuncs = new HelpFuncs();
double elapsedTime;

string programDirectory = "files";

if ( Directory.Exists(programDirectory) ) {
    Directory.Delete(programDirectory, true);
}
Directory.CreateDirectory(programDirectory);

string binFileNameInput = programDirectory + @"\\GeneretedFile.bin";
string binFileNameOutput = programDirectory + @"\\SortedFile.bin";
string txtFileNameInput = programDirectory + @"\\GeneretedFile_converted.txt";
string txtFileNameOutput = programDirectory + @"\\SortedFile_converted.txt";
string helpFileNameTemplate = programDirectory + @"\\HelpFiles\\";


Console.Write($"Generating {Constants.FILE_SIZE_MBS} Mbs file...");
Console.WriteLine($" (eta - {Math.Round(Constants.FILE_SIZE_MBS * 0.008, 1)} sec)");
elapsedTime = helpFuncs.wrapperTime(
    () => helpFuncs.generateFileBinary(binFileNameInput, Constants.FILE_SIZE_MBS)
);
Console.WriteLine($"Elapsed time: {helpFuncs.convertMsecToSec(elapsedTime)} sec");

Console.Write($"Converting input binfile into text file...");
Console.WriteLine($" (eta - {Math.Round(Constants.FILE_SIZE_MBS * 0.04, 1)} sec)");
elapsedTime = helpFuncs.wrapperTime(
    () => helpFuncs.convertBinToTextFile(binFileNameInput, txtFileNameInput)
);
Console.WriteLine($"Elapsed time: {helpFuncs.convertMsecToSec(elapsedTime)} sec");

if ( Directory.Exists(helpFileNameTemplate) ) {
    Directory.Delete(helpFileNameTemplate, true);
}
Directory.CreateDirectory(helpFileNameTemplate);

Console.Write($"Sorting {Constants.FILE_SIZE_MBS} Mbs file......");
Console.WriteLine($" (eta - {Math.Round(Constants.FILE_SIZE_MBS * 0.17, 1)} sec)");
elapsedTime = helpFuncs.wrapperTime(
    () => externalSort.balancedMultiwaySort(binFileNameInput, binFileNameOutput, helpFileNameTemplate)
);
Console.WriteLine($"Elapsed time: {helpFuncs.convertMsecToSec(elapsedTime)} sec");

Console.Write($"Converting output binfile into text file...");
Console.WriteLine($" (eta - {Math.Round(Constants.FILE_SIZE_MBS * 0.04, 1)} sec)");
elapsedTime = helpFuncs.wrapperTime(
    () => helpFuncs.convertBinToTextFile(binFileNameOutput, txtFileNameOutput)
);
Console.WriteLine($"Elapsed time: {helpFuncs.convertMsecToSec(elapsedTime)} sec");
