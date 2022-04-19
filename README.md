# Silk!

Silk! is a simple and the fastest Discord bot written in C# until proven otherwise. Silk! is built on top of the [Remora.Discord](https://github.com/Nihlus/Remora.Discord) .NET Discord library, with the goal to not only be a great bot but to also fill in the gaps that some bots have. Silk! aims to be a bot your members will want to use, while not being a cookie-cutter game, moderation, or entertainment bot. Silk! is also made with large guilds in mind. Feel free to join the [Silk! Server](https://silkbot.cc/discord) to ask any questions you may have or any general support you may need. Want this bot on your server? [Feel free to invite it!](https://silkbot.cc/invite)

[![Code Quality](https://www.codefactor.io/repository/github/VTPDevelopment/Silk/badge)](https://www.codefactor.io/repository/github/VTPDevelopment/silk)  
![Code Size](https://img.shields.io/github/languages/code-size/VTPDevelopment/Silk)  
![Lines of Code](https://img.shields.io/tokei/lines/github/VTPDevelopment/Silk)  
![GitHub Closed Issues](https://img.shields.io/github/issues-closed-raw/VelvetThePanda/Silk)  
![Discord](https://img.shields.io/discord/721518523704410202)

---

## **Development**

Silk! is an open-source bot, and if you know C#, you're more than welcome to contribute! If you're on the server, and your PR gets merged, you get a nice light blue role on the server to signify your contributions. Active contributors are given a special role as well.

### **Resources**

- [Redis](https://redis.io/)
- [PostgreSQL](https://www.postgresql.org/)
- [App Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Discord Developer Portal](https://discord.com/developers)

---

### **Database**

Silk! uses [PostgreSQL](https://www.postgresql.org/) for its backend database store. So if you don't have PostgreSQL installed, make sure to install it before trying to run the Visual Studio solution.

---

### **Configuration / Secrets Management**

To manage storage of the needed `Discord Bot Token`, `Database Connection String`, and other important project configuration / settings, Silk! uses an `appSettings.json` file.

#### **Default**

#### When you clone Silk!, before running the project, you'll need to edit the template `appSettings.json` file in the following directory: `src\Silk`

For the **Database**, look for the `Persistence` section of the file, it will look something like this:

```json
"Persistence": {
    "Host": "localhost",
    "Port": "5432",
    "Database": "silk",
    "Username": "postgres",
    "Password": ""
}
```

From there, just give the `Username` key a value (database user), and the `Password` key a value (password for the Postgres user).  

For the **Discord Bot Token**, look for the `Discord` section of the file, it will look something like this:

```json
"Discord": {
    "Shards": 1,
    "ClientId": "",
    "ClientSecret": "", 
    "BotToken": ""
}
```

From there, just give the `BotToken` key a value (your Discord Bot Token). If you don't have your token off-hand, you can get it from the [Discord Developer Portal](https://discord.com/developers); Select your application once logged in, then select `Bot` in the menu, and you should be able to Reveal your token from there.

#### **App Secrets**

The alternative, and more recommended way (but still **NOT** suitable for `Production`) for managing the configuration file is using `User Secrets`

The advantage of using User Secrets, is that the file is stored in a separate location from the project tree. Because of this, those secrets aren't checked into source control.

The location of the `secrets.json` file is different between Operating Systems, but it's stored in a system-protected user profile folder on your computer.

**Windows**
- `%APPDATA%\Microsoft\UserSecrets\VelvetThePanda-SilkBot\secrets.json`

**Mac / Linux**
- `~/.microsoft/usersecrets/VelvetThePanda-SilkBot/secrets.json`

In the `Silk` project, a `UserSecretsId` property is defined in the `Silk.csproj` file which looks like this:

```xml
<UserSecretsId>VelvetThePanda-Silk</UserSecretsId>
```

The structure for the file is the same for the [Default](#default) - `appSettings.json` approach. Copy what's in that template and paste it in the `secrets.json` file. Then fill in the needed pieces and you're good to go!

---

### Running the Project

Before you start, you will want to make sure that you have both Postgres and Redis running and available to Silk!. If the services are running in docker, you'll wanna make sure that the appropriate ports are exposed on the container (`5432` and `6375` respectively).

If Silk! is also running in Docker, ensure that it's within the same network as both Postgres and Redis. To connect, simply reference the container name instead of `localhost` or an IP (e.g. `Server=postgres`).

If you're starting fresh, just cloned the repo, then you'll need to make sure you've done the needed [Configuration](#configuration--secrets-management), before running or debugging Silk!.

If you're already using Silk!, you may need to do any or all of the following to ensure that you have everything you need to run the latest and greatest!

Silk! will automatically apply any applicable migrations to the database. If you're starting with a clean database, this may take a few seconds.

---

## Self-hosting

### Self-hosting Silk! is easy. Instructions have been provided to self-host locally.

### Docker

To run Silk! as a docker container, you can simply create an `appSettings.json` file in the root directory, and run `docker pull velvetthepanda/silk`, assuming you have a postgres database running already. If not, you can download a pre-configured `docker-compose` file [here](https://files.velvetthepanda.dev/docker-compose.yml).

The latest build can be viewed on [Docker Hub](https://hub.docker.com/r/velvetthepanda/silk/tags)

![Silk! Logo](https://files.velvetthepanda.dev/silk.png)