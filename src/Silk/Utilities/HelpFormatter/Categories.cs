﻿using System.Collections.Generic;

namespace Silk.Utilities.HelpFormatter;

public static class Categories
{
    public const string
        Dev           = "`✏️ Developer`",
        Mod           = "`⚒️ Mod`",
        General       = "`📁 General`",
        Games         = "`🎮 Games`",
        Misc          = "`💡 Misc`",
        Server        = "`🖥️ Server`",
        Bot           = "`🤖 Bot`",
        Economy       = "`💰 Economy`",
        Uncategorized = "`❓ Uncategorized`";

    public static readonly IReadOnlyList<string> Order = new[] { Dev, General, Games, Misc, Mod, Server, Bot, Economy, Uncategorized };
}

public static class CustomEmoji
{
    public const string
        Check   = "<:check:410612082929565696>",
        Cross   = "<:cross:410612082988285952>",
        Loading = "<a:loading:410612084527595520>",
        Help    = "<:help:438481218674229248>",
        Bot     = "<:bot:777726275161817088>",
        Money   = "<:money:777725904758505482>",
        Discord = "<:developer:777724802793996298>",
        GitHub  = "<:github:409803419717599234>",
        Staff   = "<:DiscordStaff:777722613966438442>",
        Server  = "<:Server:787537636007870485>",
        Empty   = "<:Empty:445680384592576514>";
}