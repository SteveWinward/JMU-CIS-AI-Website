# NonProfitSite - Local & Codespace Testing Guide

This is a minimal ASP.NET Core MVC application implementing the requirements from the project requirements document.

## Prerequisites

- .NET 8 SDK (install instructions: https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- (Optional) GitHub Codespaces account for remote testing

## Run locally (development)

1. Open a terminal in the `src/NonProfitSite` folder.
2. Restore and run the app:

   dotnet restore
   dotnet run

3. The app listens on http://0.0.0.0:5000. Open http://localhost:5000 in your browser.

Notes:
- On first run the SQLite database `app.db` will be created in the application folder and seeded with a default admin user.
- Default admin credentials: email `admin@example.org` password `Admin!123`

## Run in GitHub Codespaces / Ports

When running inside a Codespace, ensure the application listens on all network interfaces (the project is configured to do that by setting `app.Urls.Add("http://0.0.0.0:5000")`).

Steps:
1. Open the Codespace for this repository and open a terminal in `src/NonProfitSite`.
2. Run `dotnet run`.
3. In the Codespaces UI, forward port `5000` (or use the Automatic Port Detection). Expose it as public if you want to view from outside.
4. Codespaces will provide a URL (or use the forwarded port preview). Open that URL in your local browser to test the site.

## Testing Scenarios

- Public pages (no login): Home, About, Events, Contact.
- Submit feedback via Contact -> submit form. Feedback is stored in the database but not publicly visible.
- Admin: visit `/Admin/Login` and login with seeded credentials. From the dashboard: View and reply to feedback. Replies are sent using a console email sender that logs messages to application logs (so you can view them in the terminal).

## Production Notes

- Replace `ConsoleEmailSender` with an implementation that sends real emails (e.g., SendGrid).
- Add stronger authentication (consider ASP.NET Core Identity) and role management for production.
- Use proper logging, configuration, and migrations for production environments.
- Add tests and CI pipeline.

## Troubleshooting

- If port 5000 is already in use, change the port in `Program.cs` and forward the new port in Codespaces.
- If migrations or DB issues occur, delete `app.db` and re-run to recreate the seeded DB (for development only).

## Running tests

From the repository root or inside Codespaces, run tests with:

  cd src/NonProfitSite.Tests
  dotnet test
