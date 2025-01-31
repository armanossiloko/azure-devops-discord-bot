# Azure DevOps Discord Bot

Azure DevOps Bot is a Discord bot built using .NET 9 that integrates with Azure DevOps to provide slash commands for interacting with backlog items, configuring organization links, and removing them. Additionally, the bot includes an API endpoint to send notifications about pull request creations directly to Discord channels.

## Features

### Slash Commands:
- `/tfs items <list_of_item_ids>`: Fetch and display details of specified Azure DevOps backlog items. Provide a comma or space-separated list of backlog item IDs as input.
- `/link name <organization_display_name> organization-url <URL> token <PAT>`: Link an Azure DevOps organization to the current Discord server.
- `/unlink <organization_display_name>`: Unlink the Azure DevOps organization from the Discord server.
- `/subscribe`: Allow Azure DevOps web hook subscriptions sent to `/api/notifications/*` to be processed as Discord embed messages in one or multiple text channels.

### API Endpoint(s):
- **POST /api/notifications/pulls/created**: When triggered, this endpoint sends a notification to a specified Discord channel about a newly created Azure DevOps pull request.
- **POST /api/notifications/pulls/updated**: When triggered, this endpoint sends a notification to a specified Discord channel about a pull request being updated.
- **POST /api/notifications/pulls/merged**: When triggered, this endpoint sends a notification to a specified Discord channel about a pull request being merged into its target branch.
- **POST /api/notifications/pulls/commented**: When triggered, this endpoint sends a notification to a specified Discord channel about a comment posted on a pull request.

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/)
- [Discord bot token](https://discord.com/developers/applications) with proper permissions for slash commands.
- [Azure DevOps Personal Access Token (PAT)](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate) for API access.

### Installation

1. Clone the repository:

```bash
git clone https://github.com/armanossiloko/azure-devops-discord-bot.git
cd azure-devops-discord-bot
```

2. Configure environment variables by creating an `.env.override` file (the name is predefined in the `docker-compose.yml`). There you can overwrite any of the variables set in the `.env` file, for example:
```
APPLICATION__DISCORDTOKEN="<hello world>"
APPLICATION__DATABASEPROVIDER="Npgsql"

# Set the Postgres connection string to that of the Postgres container
CONNECTIONSTRINGS__NPGSQL="Host=discord-db;Port=5432;Database=discorddb;Username=discord;Password=discord;"

# This will expose the Postgres database at port 25432
PUBLIC_POSTGRES_PORT=25432

# This will expose the .NET API at port 50000, making it accessible at http://localhost:50000/
PUBLIC_BOT_PORT=50000
```

3. Boot everything up by using a `docker-compose` command and providing the `--env-file` so that the public port mapping can be set:

```bash
docker-compose --env-file ./.env.override up -d
```

4. With the given `.env.override` example before, you should now be able to access the API via http://localhost:50000/swagger/


### Usage

#### Slash Commands

1. **/link**  
Links the current Discord server with your Azure DevOps organization, allowing the bot to interact with the organization. You must have proper permissions within both Azure DevOps and Discord.

2. **/unlink**  
Use this command to remove an existing link between the current Discord server and the Azure DevOps organization.

3. **/tfs items**  
   Use this command in any Discord channel to display details of specific Azure DevOps backlog items. Example usage - this will retrieve and display details for items with IDs 123, 456, and 789:
```
/tfs items 123,456,789
```

4. **/subscribe**  
Use this command to allow an Azure DevOps web hook subscription notification to be handled by the API in order to receive a Discord notification.

#### API Webhook for Pull Request Notifications

To enable Azure DevOps to send pull request notifications to a Discord channel, follow these steps:

1. In your Azure DevOps project, navigate to **Project Settings** > **Service Hooks**.
2. Create a new web hook subscription, for example: **Pull Request Created** events.
3. Set the webhook URL to:

```
https://<your-api-server>/api/notifications/pulls/created
```

4. Call `/subscribe` in one of your Discord text channels (you first need to have an organization linked using `/link`) and subscribe to an event type (e.g `git.pullrequest.created`).

Once a pull request is created, Azure DevOps will send an HTTP request to the given endpoint, and the Discord bot is expected to handle that by sending a notification messaged in the Discord channel specified in the `/subscribe` command

---


## Credits

This project utilizes the following open-source libraries, please star them on Github and contribute to them, if able to:
- [Bruno](https://github.com/usebruno/bruno) - An open-source IDE for exploring and testing APIs (an alternative to the likes of Postman and Insomnia).
- [Discord.NET](https://github.com/discord-net/Discord.Net): An unofficial .NET wrapper for the Discord API (https://discord.com/).
- [System.IO.Abstractions](https://github.com/TestableIO/System.IO.Abstractions): Just like System.Web.Abstractions, but for System.IO. Yay for testable IO access!
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore): Swagger tools for documenting API's built on ASP.NET Core.


## Contributing

Got suggestions, found a bug or would like to contribute? If so, feel free to create issues or pull requests.
Please do make sure to:

- Follow coding standards.
- Write clear and concise commit messages.
- Add tests if containing more complex logic (e.g multiple event types handled by a single endpoint).

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
