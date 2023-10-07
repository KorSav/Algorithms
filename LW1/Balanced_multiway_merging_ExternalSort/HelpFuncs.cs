using System.Diagnostics;
using ProgramConfiguration;

namespace nmspcHelpFuncs {
    internal class HelpFuncs {

        public int convertMbsToBytes( int Mbs ) {
            return Mbs * 1024 * 1024;
        }

        public double wrapperTime( Action action ) {
            Stopwatch sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public double convertMsecToSec( double msec ) {
            return Math.Round(msec / 1000.0, 3);
        }


        /// <summary>
        /// Generates binary file, that consists of random integers 32 bits each
        /// </summary>
        /// <param name="fileName">will be filled with random integers</param>
        /// <param name="sizeMbs">result size of file in megabytes</param>
        public void generateFileBinary( string fileName, int sizeMbs ) {
            FileStream fileStream;
            BufferedStream bufStream;
            BinaryWriter binWriter;
            int freeSpace;
            long fileSize;
            int curN;
            Random random;

            fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            freeSpace = Convert.ToInt32(getFreeBytes());
            if ( freeSpace > Constants.FILE_MAX_BUFFER_SIZE ) {
                freeSpace = Constants.FILE_MAX_BUFFER_SIZE;
            }
            bufStream = new BufferedStream(fileStream, freeSpace);
            binWriter = new BinaryWriter(bufStream);
            random = new Random();
            fileSize = convertMbsToBytes(sizeMbs) / 4;
            for ( long i = 0; i < fileSize; i++ ) {
                curN = random.Next(Int32.MinValue, Int32.MaxValue);
                binWriter.Write(curN);
            }
            binWriter.Close();
            GC.Collect();
            return;
        }

        public void convertBinToTextFile( string binFileName, string txtFileName ) { 
            FileStream fsFileTxt;
            FileStream fsFileBin;
            long freeSpace;
            int bufSize;
            int currentNumber;

            fsFileBin = new FileStream(binFileName, FileMode.Open, FileAccess.Read);
            fsFileTxt = new FileStream(txtFileName, FileMode.Create, FileAccess.Write);
            freeSpace = getFreeBytes();
            bufSize = Convert.ToInt32(freeSpace / 2);
            if ( bufSize > Constants.FILE_MAX_BUFFER_SIZE ) {
                bufSize = Constants.FILE_MAX_BUFFER_SIZE;
            }
            BufferedStream bsFileTxt = new BufferedStream(fsFileTxt, bufSize);
            BufferedStream bsFileBin = new BufferedStream(fsFileBin, bufSize);
            BinaryReader binReader = new BinaryReader(bsFileBin);
            StreamWriter txtWriter = new StreamWriter(bsFileTxt);
            while ( binReader.BaseStream.Position < binReader.BaseStream.Length ) {
                currentNumber = binReader.ReadInt32();
                txtWriter.Write($"{currentNumber}\n");
            }
            txtWriter.Close();
            binReader.Close();
            GC.Collect();
            return;
        }
        
        
        /// <summary>
        /// Calcuates the amount of remained unused process memory
        /// </summary>
        /// <returns>Memory size in bytes</returns>
        public long getFreeBytes() {
            return convertMbsToBytes(Constants.RAM_SIZE_MBS - Constants.EXTRA_PROCESS_MEMORY_MBS) - GC.GetTotalMemory(false);
        }
    }
}
