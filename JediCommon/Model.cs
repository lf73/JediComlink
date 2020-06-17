namespace JediCommon
{
    public class Model
    {
        public string ModelNumber { get; set; }
        public string ModelName { get; set; }
        public string Band { get; set; }
        public string Description { get; set; }
        public bool IsFlashPort { get; set; }
        public override string ToString()
        {
            return ModelNumber;
        }
    }
}
