namespace PS3DumpChecker {
    internal sealed class ListBoxItem {
        private readonly int _id;
        private readonly string _name;

        public ListBoxItem(int id, string name) {
            _id = id;
            _name = name;
        }

        public int Value {
            get { return _id; }
        }

        public override string ToString() {
            return _name;
        }
    }
}