using InnerSortingAlgorithm;
using nmspcHelpFuncs;
using ProgramConfiguration;

namespace ExternalSortingAlgorithm {
    internal class ExternalSort {
        HelpFuncs helpFuncs = new HelpFuncs();

        /// <summary>
        /// The first action in sorting algorithm.
        /// Finds series in input file and writes them into help files
        /// </summary>
        /// <param name="fsInput">The file that will be splitted into help files</param>
        /// <param name="fsOutputArray">The files that will contain series got from input file</param>
        /// <param name="fileBufferSize">Free process memory, that can be used to buffer file streams</param>
        private void splitInputFileIntoFiles( ref FileStream fsInput, ref Span<FileStream> fsOutputArray, int fileBufferSize ) {

            int prevNum = int.MinValue;
            int curNum;
            int curBwIndex = 0;
            BufferedStream bsInput = new BufferedStream(fsInput, fileBufferSize);
            BinaryReader binReader = new BinaryReader(bsInput);
            BufferedStream[] bsOutputArray = new BufferedStream[fsOutputArray.Length];
            BinaryWriter[] binWriterArray = new BinaryWriter[fsOutputArray.Length];
            for ( int i = 0; i < fsOutputArray.Length; i++ ) {
                bsOutputArray[i] = new BufferedStream(fsOutputArray[i], fileBufferSize);
                binWriterArray[i] = new BinaryWriter(bsOutputArray[i]);
            }

            do {
                curNum = binReader.ReadInt32();
                //check if seria continues
                if ( curNum >= prevNum ) {
                    binWriterArray[curBwIndex].Write(curNum);
                    prevNum = curNum;
                    continue;
                }
                //seria ended => switch for another file
                if ( ++curBwIndex == Constants.NUM_OF_MERGE_WAYS ) {
                    curBwIndex = 0;
                }
                binWriterArray[curBwIndex].Write(curNum);
                prevNum = curNum;
            } while ( binReader.BaseStream.Position < binReader.BaseStream.Length );

            foreach ( BinaryWriter bw in binWriterArray ) {
                bw.Close();
            }
            GC.Collect();
            return;
        }


        /// <summary>
        /// Optimized version of <see cref="splitInputFileIntoFiles(ref FileStream, ref Span{FileStream}, int)">
        /// Reads chunks of max possible size from file and sorts them using inner sort
        /// </summary>
        /// <param name="sortBufferSize">Max possible chunk size in bytes</param>
        private void optimizeSplitInputFileIntoFiles( ref FileStream fsInput, ref Span<FileStream> fsOutputArray, int fileBufferSize, int sortBufferSize ) {
            int i;
            int eobuffer;
            int curBwIndex;
            InnerSort sort = new InnerSort();
            BufferedStream bsInput;
            BufferedStream[] bsOutputArray;
            int[] bufferSort = new int[sortBufferSize / 4];
            BinaryReader binReader;
            BinaryWriter[] binWriterArray = new BinaryWriter[fsOutputArray.Length];

            //init file buffers
            bsInput = new BufferedStream(fsInput, fileBufferSize);
            bsOutputArray = new BufferedStream[fsOutputArray.Length];
            for ( i = 0; i < fsOutputArray.Length; i++ ) {
                bsOutputArray[i] = new BufferedStream(fsOutputArray[i], fileBufferSize);
            }
            //init binary reader and writers
            binReader = new BinaryReader(bsInput);
            for ( i = 0; i < Constants.NUM_OF_MERGE_WAYS; i++ ) {
                binWriterArray[i] = new BinaryWriter(bsOutputArray[i]);
            }

            double quickSortTime = 0;
            curBwIndex = 0;
            do {
                eobuffer = bufferSort.Length;
                //read chunk from input file
                for ( i = 0; i < bufferSort.Length; i++ ) {
                    // check if size of remained data in file is lower than buffer size
                    if ( binReader.BaseStream.Position >= binReader.BaseStream.Length ) {
                        eobuffer = i;
                        break;
                    }
                    bufferSort[i] = binReader.ReadInt32();
                }
                quickSortTime += helpFuncs.wrapperTime(
                    () => sort.QuickSort(ref bufferSort, 0, eobuffer - 1)
                    ) / 1000;
                //write sorted chunk into curBwIndex file
                for ( i = 0; i < eobuffer; i++ )
                    binWriterArray[curBwIndex].Write(bufferSort[i]);
                //cycle curBwIndex files
                if ( ++curBwIndex == Constants.NUM_OF_MERGE_WAYS )
                    curBwIndex = 0;
            } while ( binReader.BaseStream.Position < binReader.BaseStream.Length );

            binReader.Close();
            foreach ( BinaryWriter bw in binWriterArray ) {
                bw.Close();
            }
            Console.WriteLine($"Time used for quickSort: {Math.Round(quickSortTime, 2)} s");
            GC.Collect();
            return;
        }


        /// <summary>
        /// main operation in process of sorting:
        /// merges elements from readable group of files into writable group of files
        /// </summary>
        /// <param name="binReaderArray">Files to read from</param>
        /// <param name="binWriterArray">Files to write to</param>
        /// <param name="states">Array that helps to monitor states of files to read from</param>
        private void multiwayMerging( ref BinaryReader[] binReaderArray, ref BinaryWriter[] binWriterArray, ref FileReadingState[] states ) {
            int i;
            int minFileIndex;
            int curMergedFileIndex;
            bool isCurMergedFileFilled;
            int minValue;
            int curValue;
            int[] previousElements = new int[Constants.NUM_OF_MERGE_WAYS];
            long readerPosition;

            curMergedFileIndex = 0;
            do {

                for ( i = 0; i < Constants.NUM_OF_MERGE_WAYS; i++ ) {
                    previousElements[i] = int.MinValue;
                    if ( binReaderArray[i].BaseStream.Length == 0 )
                        states[i] = FileReadingState.end;
                    else if ( states[i] != FileReadingState.end )
                        states[i] = FileReadingState.readable;
                }
                //merging into curMergedFileIndex file
                do {
                    minValue = int.MaxValue;
                    minFileIndex = Constants.NUM_OF_MERGE_WAYS;
                    //find min value
                    for ( i = 0; i < binReaderArray.Length; i++ ) {
                        if ( states[i] != FileReadingState.readable )
                            continue;
                        //peek int16
                        readerPosition = binReaderArray[i].BaseStream.Position;
                        curValue = binReaderArray[i].ReadInt32();
                        binReaderArray[i].BaseStream.Position = readerPosition;
                        //check if section end
                        if ( previousElements[i] > curValue ) {
                            states[i] = FileReadingState.unreadable;
                            continue;
                        }
                        //update minimum value
                        if ( curValue > minValue )
                            continue;
                        minValue = curValue;
                        minFileIndex = i;
                    }
                    //check if all possible sections to be merged in file ended
                    isCurMergedFileFilled = true;
                    foreach ( FileReadingState value in states ) {
                        if ( value != FileReadingState.readable )
                            continue;
                        isCurMergedFileFilled = false;
                        break;
                    }
                    if ( isCurMergedFileFilled ) {
                        curMergedFileIndex++;
                        break;
                    }

                    //write min value to file buffer and read it from file where it was
                    binWriterArray[curMergedFileIndex].Write(minValue);
                    binReaderArray[minFileIndex].ReadInt32();
                    if ( binReaderArray[minFileIndex].BaseStream.Position == binReaderArray[minFileIndex].BaseStream.Length ) {
                        states[minFileIndex] = FileReadingState.end;
                    }
                    previousElements[minFileIndex] = minValue;

                } while ( !isCurMergedFileFilled );

                //cycle files for merging
                if ( curMergedFileIndex == Constants.NUM_OF_MERGE_WAYS ) {
                    curMergedFileIndex = 0;
                }
            } while ( !isAllStatesEnd(ref states) );
            GC.Collect();
            return;
        }


        /// <summary>
        /// Sorts binary file using balanced multiway version of sorting algorithm
        /// </summary>
        /// <param name="fileNameInput">To be sorted</param>
        /// <param name="fileNameOutput">Will contain sorted version of input file</param>
        /// <param name="fileNameHelpTemplate">Template of help files name that will be created</param>
        public void balancedMultiwaySort( string fileNameInput, string fileNameOutput, string fileNameHelpTemplate ) {
            int bufferSizeForFile;
            int bufferSizeForSort;
            long freeSpace;
            FileStream[] fsHelpArray = new FileStream[Constants.NUM_OF_MERGE_WAYS * 2];
            FileReadingState[] states = new FileReadingState[Constants.NUM_OF_MERGE_WAYS];
            Span<FileStream> fsHelpArraySpan;
            int i;

            //initialization of filestreams 
            FileStream fsInput = new FileStream(fileNameInput, FileMode.Open, FileAccess.Read);
            for ( i = 0; i < Constants.NUM_OF_MERGE_WAYS; i++ ) {
                fsHelpArray[i] = new FileStream($"{fileNameHelpTemplate}{i}.bin", FileMode.Create, FileAccess.Write);
            }

            //initialization of buffers 
            freeSpace = helpFuncs.getFreeBytes();
            bufferSizeForFile = Convert.ToInt32(freeSpace / ( Constants.NUM_OF_MERGE_WAYS + 2 ));
            if ( bufferSizeForFile > Constants.FILE_MAX_BUFFER_SIZE ) {
                bufferSizeForFile = Constants.FILE_MAX_BUFFER_SIZE;
            }
            bufferSizeForSort = Convert.ToInt32(freeSpace - bufferSizeForFile * ( Constants.NUM_OF_MERGE_WAYS + 1 ));
            if ( bufferSizeForSort / 1024 / 1024 > Constants.FILE_SIZE_MBS ) {
                bufferSizeForSort = Convert.ToInt32(helpFuncs.convertMbsToBytes(Constants.FILE_SIZE_MBS));
            }

            fsHelpArraySpan = new Span<FileStream>(fsHelpArray, 0, Constants.NUM_OF_MERGE_WAYS);
            if ( Constants.OPTIMIZED )
                optimizeSplitInputFileIntoFiles(ref fsInput, ref fsHelpArraySpan, bufferSizeForFile, bufferSizeForSort);
            else
                splitInputFileIntoFiles(ref fsInput, ref fsHelpArraySpan, bufferSizeForFile);
            GC.Collect();

            //getting free space amount for buffers
            freeSpace = helpFuncs.getFreeBytes();
            bufferSizeForFile = Convert.ToInt32(freeSpace / Constants.NUM_OF_MERGE_WAYS * 2);
            if ( bufferSizeForFile > Constants.FILE_MAX_BUFFER_SIZE ) {
                bufferSizeForFile = Constants.FILE_MAX_BUFFER_SIZE;
            }
            BufferedStream[] bs_HelpArray = new BufferedStream[fsHelpArray.Length];
            BinaryReader[] br_HelpArray = new BinaryReader[Constants.NUM_OF_MERGE_WAYS];
            BinaryWriter[] bw_HelpArray = new BinaryWriter[Constants.NUM_OF_MERGE_WAYS];
            bool isMergeSecond = false;
            bool nextMerge;
            initMergeFileStreams(ref fsHelpArray, ref fileNameHelpTemplate, 0, Constants.NUM_OF_MERGE_WAYS);
            //cycle of all multiway merges
            do {
                initMergeBufferedStreams(ref fsHelpArray, ref bs_HelpArray, bufferSizeForFile);
                initMergeBinaryProcessors(ref br_HelpArray, ref bw_HelpArray, ref bs_HelpArray);

                multiwayMerging(ref br_HelpArray, ref bw_HelpArray, ref states);

                for ( i = 0; i < states.Length; i++ )
                    states[i] = FileReadingState.readable;
                for ( i = 0; i < Constants.NUM_OF_MERGE_WAYS; i++ ) {
                    bw_HelpArray[i].Close();
                    br_HelpArray[i].Close();
                }
                GC.Collect();
                isMergeSecond = !isMergeSecond;
                if ( isMergeSecond ) {
                    initMergeFileStreams(ref fsHelpArray, ref fileNameHelpTemplate,
                        Constants.NUM_OF_MERGE_WAYS, fsHelpArray.Length);
                    nextMerge = ! isFilesEmpty(ref fsHelpArray,
                        Constants.NUM_OF_MERGE_WAYS + 1, fsHelpArray.Length);
                } else {
                    initMergeFileStreams(ref fsHelpArray, ref fileNameHelpTemplate,
                        0, Constants.NUM_OF_MERGE_WAYS);
                    nextMerge = ! isFilesEmpty(ref fsHelpArray,
                        1, Constants.NUM_OF_MERGE_WAYS);
                }

            } while ( nextMerge );
            isMergeSecond = !isMergeSecond;

            //closing streams and creating output file
            foreach ( FileStream fs in fsHelpArray ) {
                fs.Close();
            }
            if ( File.Exists(fileNameOutput) ) {
                File.Delete(fileNameOutput);
            }
            if ( isMergeSecond ) {
                File.Move($"{fileNameHelpTemplate}0.bin", fileNameOutput);
            } else {
                File.Move($"{fileNameHelpTemplate}{Constants.NUM_OF_MERGE_WAYS}.bin", fileNameOutput);
            }
            return;
        }


        /// <summary>
        /// Initializes binary read and write array with an apppropriate buffered streams
        /// </summary>
        private void initMergeBinaryProcessors( ref BinaryReader[] binReaderArray, ref BinaryWriter[] binWriterArray, ref BufferedStream[] bsArray) {
            int brIndex = 0;
            int bwIndex = 0;
            foreach ( BufferedStream bs in bsArray) {
                if ( bs.CanWrite )
                    binWriterArray[bwIndex++] = new BinaryWriter(bs);
                else
                    binReaderArray[brIndex++] = new BinaryReader(bs);
            }
            return;
        }


        /// <summary>
        /// Initializes filestreams for reading by given span and other for writing
        /// </summary>
        /// <param name="fsArray">Filestreams that should be initialized</param>
        /// <param name="fileNameHelpTemplate">Template for files names</param>
        /// <param name="from">The beginner index of span for read</param>
        /// <param name="to">Last index for span for read (not including)</param>
        private void initMergeFileStreams( ref FileStream[] fsArray, ref string fileNameHelpTemplate, int from, int to) {
            for ( int i = 0; i < fsArray.Length; i++ ) {
                if ( i >= from && i < to ) {
                    fsArray[i] = new FileStream($"{fileNameHelpTemplate}{i}.bin", FileMode.Open, FileAccess.Read);
                } else {
                    fsArray[i] = new FileStream($"{fileNameHelpTemplate}{i}.bin", FileMode.Create, FileAccess.Write);
                }
            }
            return;
        }


        /// <summary>
        /// Initializes buffered streams of given size from file streams
        /// </summary>
        private void initMergeBufferedStreams( ref FileStream[] fsArray, ref BufferedStream[] bsArray, int bufSize ) {
            for ( int i = 0; i < bsArray.Length; i++ ) {
                bsArray[i] = new BufferedStream(fsArray[i], bufSize);
            }
        }


        /// <summary>
        /// Checks if file streams in span are empty
        /// </summary>
        /// <param name="fsArray">File streams array</param>
        /// <param name="from">Beggining of span</param>
        /// <param name="to">End of span (not including)</param>
        /// <returns>true if files are empty, otherwise false</returns>
        public bool isFilesEmpty( ref FileStream[] fsArray, int from, int to) {
            for ( int i = from; i < to; i++ ) {
                if ( fsArray[i].Length != 0 ) {
                    return false;
                }
            }
            return true;
        }

        private enum FileReadingState : byte {
            /// <summary>
            /// Set if seria continues, go on reading
            /// </summary>
            readable,
            /// <summary>
            /// Set if seria ended, stop reading (can be read on merging into next file)
            /// </summary>
            unreadable,
            /// <summary>
            /// Set if file is empty, stop reading (can't be read on merging into next file)
            /// </summary>
            end
        }
        private bool isAllStatesEnd( ref FileReadingState[] states ) {
            foreach ( FileReadingState state in states ) {
                if ( state != FileReadingState.end )
                    return false;
            }
            return true;
        }

    }
}
