# Naloga 1
Small .NET core application (API), which does the following:
- parse RSS feed
- extract statistics from the feed
- expose endpoint for reading the statistics
- expose endpoint for uploading new RSS feed

No special documentation is added since the app is pretty trivial.

## Quick start
When application is started in VsCode, file `rss.xml` is loaded from the local filesystem.
Application may be started using the launch scheme found in `.vscode` folder.

Make sure you have the development root cert correctly set-up, when testing using the browser. [See here for details.](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos)

After the application has started there is route `NewsStatistics` available on the `localhost`.


**Get news statistics:**
        
    curl -i https://localhost:5001/NewsStatistics

**Upload new RSS feed:**
        
    curl -X POST --data-binary @rss.xml https://localhost:5001/NewsStatistics
where `rss.xml` is the feed to upload.
