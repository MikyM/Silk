using System.Collections.Generic;
using OneOf;
using Remora.Commands.Trees.Nodes;
using Remora.Discord.API.Abstractions.Objects;

namespace Silk.Utilities.HelpFormatter;

/// <summary>
///     Represents a utility class for generating an embed to display in a help command.
/// </summary>
public interface IHelpFormatter
{
	/// <summary>
	///     Used to format a message representing a single command.
	/// </summary>
	/// <param name="command">The command to show a help message for.</param>
	/// <returns>An embed displaying help for a single command.</returns>
	public IEnumerable<IEmbed> GetCommandHelpEmbeds(OneOf<CommandNode, IReadOnlyList<CommandNode>> command);

	/// <summary>
	///     Used to format a message representing a group of commands. Any of the nodes passed to the method may be a group node.
	/// </summary>
	/// <param name="subcommands">The subcommands of the parent.</param>
	/// <returns>An embed displaying help for a group of commands.</returns>
	public IEmbed GetSubcommandHelpEmbed(IEnumerable<IChildNode> subcommands);
}