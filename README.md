# ThemeSwitcher
Switch the SXA themes of the Sitecore Site based on the cookie value

Please read through this article to understand the implementation of Theme switcher functionality.

Article Url: 

# PreRequisites
•	Sitecore 10.2 with SXA installed in local

•	Developer workstation to validate the solution

# Setup
1. Clone the below repository or download the zip file at your local machine drive.
	https://github.com/jitendrachilate/ThemeSwitcher
2. Take the backup of webroot/bin folder.
3. Add the ThemeSwitcher .csproj into existing visual studio solution and build the solution.
4. Make the neccessary changes inside the CustomThemingContext.cs file as per the themes available in Sitecore instance and build the solution.
5. We can have multiple cookie values if there are more than two themes and can use switch statement to return the theme as per the cookie value.
6. Copy the dll ThemeSwitcher.dll into webroot/bin folder though through auto-publish the dll would 
be placed at webroot/bin folder when build the solution if the auto-publish is already configured.
