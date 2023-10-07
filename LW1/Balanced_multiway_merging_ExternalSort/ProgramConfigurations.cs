
using System.Configuration;

namespace ProgramConfiguration {
    internal class Constants {
        public const int RAM_SIZE_MBS = 128; // must be greater than extra_process_memory
        public const int FILE_SIZE_MBS = 256;
        public const int NUM_OF_MERGE_WAYS = 3;
        public const int EXTRA_PROCESS_MEMORY_MBS = 12; // 14 for debug, 12 for release
        public const bool OPTIMIZED = true;
        public const int FILE_MAX_BUFFER_SIZE = 4 * 1024; // more than 4kb don't increase in speed

    }
}
