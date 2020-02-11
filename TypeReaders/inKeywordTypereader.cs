using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace BJR_bot.TypeReaders
{
    public class InKeywordTypereader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (input == "in" || input == "dans")
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new InKeywordType()));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.UnknownCommand,
                "Parsing of the command failed"));
        }
    }
}
