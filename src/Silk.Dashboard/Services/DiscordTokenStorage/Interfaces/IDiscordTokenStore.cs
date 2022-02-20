﻿namespace Silk.Dashboard.Services.DiscordTokenStorage.Interfaces;

public interface IDiscordTokenStore
{
    bool                     SetToken(string    userId, IDiscordTokenStoreEntry? token);
    IDiscordTokenStoreEntry? GetToken(string    userId);
    bool                     RemoveToken(string userId);
    void                     RemoveAllTokens();
}