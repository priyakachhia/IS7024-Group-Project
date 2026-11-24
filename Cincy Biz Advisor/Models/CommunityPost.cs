using System.ComponentModel.DataAnnotations;

namespace Cincy_Biz_Advisor.Models;

public class CommunityPost
{
    public int Id { get; set; }

    [MaxLength(120)]
    public required string Title { get; set; }

    [MaxLength(100)]
    public required string Neighborhood { get; set; }

    [MaxLength(50)]
    public required string BusinessType { get; set; }

    [MaxLength(50)]
    public required string Category { get; set; }

    [MaxLength(2000)]
    public required string Message { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}