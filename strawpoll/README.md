## Migrations
Create a new migration:

`dotnet ef migrations add <name>`

Apply migrations:

`dotnet ef database update`

## Mails
For debugging purposes, mails will be sent to your [mailtrap](https://mailtrap.io/) inbox. Edit `appsettings.json` to change this.