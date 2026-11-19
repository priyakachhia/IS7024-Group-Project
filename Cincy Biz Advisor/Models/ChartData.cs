
/// <summary>One point on a monthly trend line (first day of the month and its count).</summary>
public sealed record MonthlyCount(DateOnly Month, int Count);

/// <summary>One slice of the crime-by-type breakdown.</summary>
public sealed record CategoryCount(string Category, int Count);