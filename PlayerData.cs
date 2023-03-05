namespace CustomSlot {
    public struct PlayerData<T> {
        public string Tag;
        public T Value;

        public PlayerData(string tag, T value) {
            Tag = tag;
            Value = value;
        }
    }
}
