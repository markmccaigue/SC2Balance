namespace SC2Balance.Models
{
    public class PostProcessingOutput
    {
        public int Id { get; set; }

        public int ProcessingRunId { get; set; }
        public virtual ProcessingRun ProcessingRun { get; set; }

        public string PostProcessingJobType { get; set; }
        public string JsonResults { get; set; }
    }
}
