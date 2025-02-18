namespace Utility.Nodes.Demo.Infrastructure
{
    public class LedModel
    {
        public LEDMessage Message { get; } = new LEDMessage
        {
            Channel = 0,
            //PatternIndex = 1,
            //LedFrom = 0,
            //LedTo = 0,
            //SkipFrame = 0,
            //SkipLED = 0,
            //Data = "",
        };

        public void Send()
        {

        }
    }



    public class LEDMessage
    {
        public byte Channel { get; set; }
        public int PatternIndex { get; set; }
        //public int LedFrom { get; set; }
        //public int LedTo { get; set; }
        //public int SkipFrame { get; set; }
        //public int SkipLED { get; set; }
        //public string Data { get; set; }

    }
}

