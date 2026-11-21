namespace Peytral.Observations;

public enum SeverityLevel
{
  /// <summary>
  /// Informational context or structural metadata.
  /// Indicates benign heuristics or noteworthy assembly characteristics (e.g., known vulnerabilities in frameworks, vulnerable data handling / serializing)
  /// that carry no security risk.
  /// </summary>
  Message,

  /// <summary>
  /// Low-to-moderate suspicion heuristic.
  /// Indicates patterns common in advanced modding (e.g. file IO in sensitive locations, embedded assemblies, unusual network use) 
  /// that are harmless on their own, but warrant context when paired with other hits.
  /// </summary>
  Warning,

  /// <summary>
  /// High-suspicion or high-risk capability detection.
  /// Indicates behaviors frequently exploited for malicious ends (e.g., unmanaged memory allocation, 
  /// P/Invoke to sensitive OS APIs, dynamic code emission / assembly generation, obfuscation). Manual code inspection is strongly recommended.
  /// </summary>
  Alert,

  /// <summary>
  /// Critical threat detection.
  /// Indicates patterns, signatures, or behavior chains that almost exclusively serve malicious intent 
  /// (e.g., known malware payloads, direct memory injection, credential harvesting, remote shellcode download). 
  /// <b>The assembly should be isolated immediately</b>.
  /// </summary>
  Critical,
}
