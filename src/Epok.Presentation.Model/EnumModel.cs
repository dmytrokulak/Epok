namespace Epok.Presentation.Model
{
    public class EnumModel
    {
        public static EnumModel New(int key, string value) => 
            new EnumModel(key, value);

        private EnumModel(int key, string value)
        {
            Key = key;
            Value = value;
        }

        public int Key { get; }
        public string Value { get; }
    }
}
