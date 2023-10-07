using ProgramConfiguration;
using ExternalSortingAlgorithm;
using nmspcHelpFuncs;

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
