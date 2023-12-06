namespace SearchInSetOfStates{
    public class SearchInfo {
        int memst;
        public int IterCount {
            get; set;
        }
        public long GenStatesCount {
            get; set;
        }
        public int MemStatesCount {
            get {
                return memst;
            }
            set {
                if ( value > memst ) {
                    memst = value;
                }
            }
        }
        public bool MemoryOverused {
            get; set;
        }
        public SearchInfo() {
            IterCount = 0;
            GenStatesCount = 0;
            memst = 0;
            MemoryOverused = false;
        }
        public SearchInfo(SearchInfo info) {
            IterCount = info.IterCount;
            GenStatesCount = info.GenStatesCount;
            memst = info.memst;
            MemoryOverused = false;
        }
        public static SearchInfo operator+(SearchInfo info1, SearchInfo info2) {
            SearchInfo result = new() {
                IterCount =
                info1.IterCount + info2.IterCount,
                GenStatesCount =
                info1.GenStatesCount + info2.GenStatesCount,
            };
            return result;
        }

        public override string ToString() {
            return $"(Amount of iterations; " +
                $"Generated states; States in memory)\n" +
                $"{IterCount}\t{GenStatesCount}\t{memst}\n";
        }
    }
}
