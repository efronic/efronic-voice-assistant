public class Options
{
    public string Tokens { get; set; }
    public string Provider { get; set; }
    public string Encoder { get; set; }
    public string Decoder { get; set; }
    public string Joiner { get; set; }
    public string ParaformerEncoder { get; set; }
    public string ParaformerDecoder { get; set; }
    public int NumThreads { get; set; }
    public string DecodingMethod { get; set; }
    public bool Debug { get; set; }
    public int SampleRate { get; set; }
    public int MaxActivePaths { get; set; }
    public bool EnableEndpoint { get; set; }
    public float Rule1MinTrailingSilence { get; set; }
    public float Rule2MinTrailingSilence { get; set; }
    public float Rule3MinUtteranceLength { get; set; }
}