namespace ClaimCommander.Models
{
    public class Document // Changed from 'struct' to 'class'
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        // Navigation property to link back to the Claim
        public virtual Claim Claim { get; set; }
    }
}