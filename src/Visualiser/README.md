# Configuration for new ASP.NET App Registration

1. New app registration in Entra
	- Supported account types: Single tenant
	- Redirect URI: Platform = web, URI = https://localhost:7123/signin-oidc
	- NOTE: You do not need to configure Implicit grant and hybrid flows
2.	Register permissions under "API Permissions" section. Configure scopes:
	- `email`
	- `openid`
	- `profile`
	- `offline_access`
3. Copy `Directory (tenant) ID` and `Application (client) ID` from the Overview page under your new app registration
4. Create a new ASP.NET web application
	`dotnet new webapp --auth SingleOrg --tenant-id "<TENANT_ID>" --client-id="<CLIENT_ID>"`
5.	Configure web application (`appsettings.json`)
	```json
	"AzureAd": {
		"Instance": "https://login.microsoftonline.com/",
		"Domain": "JusticeUK.onmicrosoft.com",
		"TenantId": "<TENANT_ID>",
		"ClientId": "<CLIENT_ID>",
		"CallbackPath": "/signin-oidc",
		// Add these sections
		"ResponseType": "code",
		"Scope": ["openid", "email", "profile", "offline_access"]
	}
	```
6.	Add a client credential (required for server-side web apps)
	- Under your new App Registration, go to **Certificates & secrets → New client secret**
		- Description: ASP.NET Web App
		- Expiry: 6–12 months for dev
		- Copy the Value (not the ID)
	- Configure Client Secret for ASP.NET app `dotnet user-secrets set "AzureAd:ClientSecret" "<SECRET_VALUE>"`
7. To restrict access to a group (e.g. **CFO - Digital**), you must:
	- Visit **Enterprise applications →** find your app (same name as app registration)
		- **Properties → User assignment required? → Yes**
		- **Users and groups → Add user/group →** select **CFO - Digital**

At this point, you will likely need to request admin consent for the app registration. To do so, 
visit the #staff-identity-authentication-services channel in Slack and complete the `App User.Read Request` process.

## Configuring API Access
8. New app registration in Entra
	- Supported account types: Single tenant
	- Redirect URI: (leave blank, not required for APIs)
9. Expose the API
	- App registrations → *your_api* → Expose an API
		- Set **Application ID URI**: click Add. Prefer `api://<API_CLIENT_ID>`
		- Add scopes (delegated permissions for user-based calls):
		- `api.read`
			- Scope name: `api.read`
			- Who can consent: *Admins and users*
			- Admin consent display name: *Read from API*
			- User consent display name: *Read from API*
			- State: *Enabled*
		- `api.write`
			- Scope name: `api.write`
			- Who can consent: *Admins and users*
			- Admin consent display name: *Write to API*
			- User consent display name: *Write to API*
			- State: *Enabled*
10. Grant the web app access to the API
	- App Registrations → API Permissions → Add a permission → My APIs → *your_api*
		- **Delegated permissions**:
			- [x] `api.read`
			- [x] `api.write`
	- App registrations → *your_api* → Expose an API → Authorized client applications → Add a client application
		- Client ID: *enter the Application (Client) ID of the web application*
		- Authorized scopes: **select all**
