using Remora.Results;

namespace Silk.Errors;

/// <summary>
///     Represents an error that occurs when permissions are not in a suitable state.
/// </summary>
public record PermissionError(string Message) : ResultError(Message);